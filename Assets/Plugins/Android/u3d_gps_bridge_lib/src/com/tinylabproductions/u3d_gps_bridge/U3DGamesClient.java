package com.tinylabproductions.u3d_gps_bridge;

import android.content.Intent;
import android.util.Log;

import com.google.android.gms.auth.api.Auth;
import com.google.android.gms.auth.api.signin.GoogleSignIn;
import com.google.android.gms.auth.api.signin.GoogleSignInAccount;
import com.google.android.gms.auth.api.signin.GoogleSignInClient;
import com.google.android.gms.auth.api.signin.GoogleSignInOptions;
import com.google.android.gms.auth.api.signin.GoogleSignInResult;
import com.google.android.gms.auth.api.signin.GoogleSignInStatusCodes;
import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GooglePlayServicesUtil;
import com.google.android.gms.common.api.ApiException;
import com.google.android.gms.games.Games;
import com.google.android.gms.games.GamesActivityResultCodes;
import com.google.android.gms.tasks.OnSuccessListener;
import com.google.android.gms.tasks.Task;
import com.tinylabproductions.tlplib.UnityActivity;

public class U3DGamesClient {
  public static final String TAG = "U3DGamesClient";
  public static final String GPGS = "Google Play Game Services";

  private final int playServicesSupported;
  private final GoogleSignInClient client;
  private final UnityActivity activity;
  public final ConnectionCallbacks connectionCallbacks;
  private final int signInCode, achievementsCode, leaderboardCode;

  public U3DGamesClient(final ConnectionCallbacks connectionCallbacks) throws ClassNotFoundException, NoSuchFieldException, IllegalAccessException {
    activity = (UnityActivity) Class.forName("com.unity3d.player.UnityPlayer").getField("currentActivity").get(null);
    signInCode = activity.generateRequestCode();
    achievementsCode = activity.generateRequestCode();
    leaderboardCode = activity.generateRequestCode();
    this.connectionCallbacks = connectionCallbacks;

    playServicesSupported =
      GooglePlayServicesUtil.isGooglePlayServicesAvailable(activity);

    if (playServicesSupported == ConnectionResult.SUCCESS) {
      GoogleSignInOptions builder = new GoogleSignInOptions.Builder(GoogleSignInOptions.DEFAULT_SIGN_IN).requestEmail().build();
      client = GoogleSignIn.getClient(activity, builder);
    } else {
      client = null;
    }

    activity.subscribeOnActivityResult(new UnityActivity.IActivityResult() {
      @Override
      public void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == signInCode) {
          // https://stackoverflow.com/questions/35008490/android-google-plus-sign-in-issue-handlesigninresult-returns-false
          GoogleSignInResult result = Auth.GoogleSignInApi.getSignInResultFromIntent(data);
          int statusCode = result.getStatus().getStatusCode();
          if (statusCode == GoogleSignInStatusCodes.SIGN_IN_CANCELLED) {
            connectionCallbacks.onSignInCanceled();
          } else {
            //https://developers.google.com/identity/sign-in/android/
            Task<GoogleSignInAccount> task = GoogleSignIn.getSignedInAccountFromIntent(data);
            handleSignInResult(task);
          }
        } else if (requestCode == achievementsCode || requestCode == leaderboardCode) {
          switch (requestCode) {
            case GamesActivityResultCodes.RESULT_RECONNECT_REQUIRED:
              connectionCallbacks.onDisconnected();
              break;
            case GamesActivityResultCodes.RESULT_NETWORK_FAILURE:
              connectionCallbacks.onNetworkFailure();
              break;
          }
        }
      }
    });
  }

  private void handleSignInResult(Task<GoogleSignInAccount> completedTask) {
    try {
      GoogleSignInAccount account = completedTask.getResult(ApiException.class);
      // Signed in successfully
      connectionCallbacks.onSignIn();
    } catch (ApiException e) {
      // The ApiException status code indicates the detailed failure reason.
      // Please refer to the GoogleSignInStatusCodes class reference for more information.
      Log.w(TAG, "signInResult:failed code=" + e.getStatusCode());
      connectionCallbacks.onSignInFailed();
    }
  }

  public void connect() {
    Log.d(TAG, "connect()");
    Intent signInIntent = client.getSignInIntent();
    // random id
    activity.startActivityForResult(signInIntent, signInCode);
  }

  public boolean isSupported() {
    return playServicesSupported == ConnectionResult.SUCCESS;
  }

  public boolean isServiceMissing() {
    return playServicesSupported == ConnectionResult.SERVICE_MISSING;
  }

  public boolean isServiceVersionUpdateRequired() {
    return playServicesSupported ==
      ConnectionResult.SERVICE_VERSION_UPDATE_REQUIRED;
  }

  public boolean isServiceDisabled() {
    return playServicesSupported == ConnectionResult.SERVICE_DISABLED;
  }

  public boolean isServiceInvalid() {
    return playServicesSupported == ConnectionResult.SERVICE_INVALID;
  }

  public boolean isConnected() {
    return GoogleSignIn.getLastSignedInAccount(activity) != null;
  }

  public void submitScore(String leaderboardId, long score) {
    Log.d(TAG, String.format(
      "Submitting score %d to leaderboard %s", score, leaderboardId
    ));
    assertConnectivity();
    Games.getLeaderboardsClient(activity, GoogleSignIn.getLastSignedInAccount(activity)).submitScore(leaderboardId, score);
  }

  public void unlockAchievement(String achievementId) {
    Log.d(TAG, String.format("Unlocking achievement %s.", achievementId));
    assertConnectivity();
    Games.getAchievementsClient(activity, GoogleSignIn.getLastSignedInAccount(activity)).unlock(achievementId);
  }

  public boolean showAchievements() {
    if (! tryConnectivity()) {
      Log.i(TAG, String.format(
        "Cannot show achievements because %s is not connected.",
        GPGS
      ));
      return false;
    }

    Log.d(TAG, "Showing achievements.");

    Games.getAchievementsClient(activity, GoogleSignIn.getLastSignedInAccount(activity))
            .getAchievementsIntent()
            .addOnSuccessListener(new OnSuccessListener<Intent>() {
              @Override
              public void onSuccess(Intent intent) {
                activity.startActivityForResult(intent, achievementsCode);
              }
            });
    return true;
  }

  public boolean showLeaderboard(String leaderboardId) {
    if (! tryConnectivity()) {
      Log.i(TAG, String.format(
        "Cannot show leaderboard %s, because %s is not connected.",
        leaderboardId, GPGS
      ));
      return false;
    }

    Log.d(TAG, "Starting activity to show leaderboard " + leaderboardId);
    Games.getLeaderboardsClient(activity, GoogleSignIn.getLastSignedInAccount(activity))
            .getLeaderboardIntent(leaderboardId)
            .addOnSuccessListener(new OnSuccessListener<Intent>() {
              @Override
              public void onSuccess(Intent intent) {
                activity.startActivityForResult(intent, leaderboardCode);
              }
            });
    return true;
  }

  private boolean tryConnectivity() {
    if (! isConnected()) {
      connect();
      return false;
    }

    return true;
  }

  private void assertConnectivity() {
    if (!isConnected())
      throw new IllegalStateException(
        "You need to be connected to perform this operation!"
      );
  }
}
