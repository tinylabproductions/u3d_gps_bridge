package com.tinylabproductions.u3d_gps_bridge;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Intent;
import android.support.annotation.NonNull;
import android.util.Log;

import com.google.android.gms.auth.api.Auth;
import com.google.android.gms.auth.api.signin.GoogleSignIn;
import com.google.android.gms.auth.api.signin.GoogleSignInAccount;
import com.google.android.gms.auth.api.signin.GoogleSignInClient;
import com.google.android.gms.auth.api.signin.GoogleSignInOptions;
import com.google.android.gms.auth.api.signin.GoogleSignInResult;
import com.google.android.gms.auth.api.signin.GoogleSignInStatusCodes;
import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GoogleApiAvailability;
import com.google.android.gms.games.Games;
import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.OnSuccessListener;
import com.google.android.gms.tasks.Task;
import com.tinylabproductions.tlplib.ActivityResultTracker;
import com.tinylabproductions.tlplib.IActivityWithResultTracker;
import com.tinylabproductions.tlplib.UnityActivity;

public class U3DGamesClient {
  private static final String TAG = "U3DGamesClient";
  private static final String GPGS = "Google Play Game Services";

  private final int playServicesSupported;
  private final GoogleSignInClient client;
  private final Activity activity;
  private final ConnectionCallbacks connectionCallbacks;
  private final int signInCode, achievementsCode, leaderboardCode;
  private Boolean checkedConnectionOnce = false;

  public U3DGamesClient(final ConnectionCallbacks connectionCallbacks)
    throws ClassNotFoundException, NoSuchFieldException, IllegalAccessException
  {
    activity = (Activity) Class.forName("com.unity3d.player.UnityPlayer").getField("currentActivity").get(null);
    ActivityResultTracker tracker = ((IActivityWithResultTracker) activity).getTracker();
    signInCode = tracker.generateRequestCode();
    achievementsCode = tracker.generateRequestCode();
    leaderboardCode = tracker.generateRequestCode();
    this.connectionCallbacks = connectionCallbacks;


    playServicesSupported =
      GoogleApiAvailability.getInstance().isGooglePlayServicesAvailable(activity);

    if (playServicesSupported == ConnectionResult.SUCCESS) {
      GoogleSignInOptions builder = new GoogleSignInOptions
        .Builder(GoogleSignInOptions.DEFAULT_SIGN_IN)
        .requestScopes(Games.SCOPE_GAMES_LITE)
        .requestEmail()
        .build();
      client = GoogleSignIn.getClient(activity, builder);
    } else {
      client = null;
    }

    tracker.subscribeOnActivityResult(new UnityActivity.IActivityResult() {
      @Override
      public void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == signInCode) {
          GoogleSignInResult result = Auth.GoogleSignInApi.getSignInResultFromIntent(data);

          int code = result.getStatus().getStatusCode();

          Log.d(TAG, "GoogleSignInStatusCodes " + code);
          if (result.isSuccess()) {
            setViewForPopups(result.getSignInAccount());
            connectionCallbacks.onSignIn();
          } else {
            if (code == GoogleSignInStatusCodes.SIGN_IN_CANCELLED) {
              connectionCallbacks.onSignInCanceled();
            } else {
              String message = result.getStatus().getStatusMessage();
              if (message == null || message.isEmpty()) {
                message = "Google Sign-in failed";
              }
              new AlertDialog.Builder(activity).setMessage(message)
                      .setNeutralButton(android.R.string.ok, null).show();

              connectionCallbacks.onSignInFailed();
            }
          }
        } else if (requestCode == achievementsCode || requestCode == leaderboardCode) {
          Log.d(TAG, "GamesActivityResultCodes " + resultCode);
          if (GoogleSignIn.getLastSignedInAccount(activity) == null) {
            connectionCallbacks.onDisconnected();
          }
        }
      }
    });
  }

  public void connect() {
    Log.d(TAG, "connect()");
    signInSilently();
  }

  private void setViewForPopups(GoogleSignInAccount signInAccount) {
    Games
      .getGamesClient(activity, signInAccount)
      .setViewForPopups(activity.findViewById(android.R.id.content));
  }

  // https://developers.google.com/games/services/android/signin
  private void signInSilently() {
    final GoogleSignInClient signInClient =
      GoogleSignIn.getClient(activity, GoogleSignInOptions.DEFAULT_GAMES_SIGN_IN);

    signInClient.silentSignIn().addOnCompleteListener(activity,
      new OnCompleteListener<GoogleSignInAccount>() {
        @Override
        public void onComplete(@NonNull Task<GoogleSignInAccount> task) {
          Log.d(TAG, "signInSilently successful " + task.isSuccessful());
          if (task.isSuccessful()) {
            // The signed in account is stored in the task's result.
            GoogleSignInAccount signedInAccount = task.getResult();
            setViewForPopups(signedInAccount);
            connectionCallbacks.onSignIn();
          } else {
            // Player will need to sign-in explicitly using via UI
            Intent signInIntent = client.getSignInIntent();
            activity.startActivityForResult(signInIntent, signInCode);
          }
        }
      });
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
    Boolean result = GoogleSignIn.getLastSignedInAccount(activity) != null;
    if (!checkedConnectionOnce) {
      checkedConnectionOnce = true;
      if (result) connectionCallbacks.onSignIn();
    }
    return result;
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