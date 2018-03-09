package com.tinylabproductions.u3d_gps_bridge;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

import com.google.android.gms.auth.api.signin.GoogleSignIn;
import com.google.android.gms.auth.api.signin.GoogleSignInClient;
import com.google.android.gms.auth.api.signin.GoogleSignInOptions;
import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GooglePlayServicesUtil;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.games.Games;
import com.google.android.gms.tasks.OnSuccessListener;

public class U3DGamesClient {
  public static final String TAG = "U3DGamesClient";
  public static final String GPGS = "Google Play Game Services";

  // An arbitrary integer that you define as the request code.
  private static final int REQUEST_LEADERBOARD = 0;
  private static final int REQUEST_ACHIEVEMENTS = 1;

  private final int playServicesSupported;
  private final GoogleSignInClient client;
  private final Activity activity;
  public final ConnectionCallbacks connectionCallbacks;
  private final long id;

  private final GoogleApiClient.ConnectionCallbacks gpscCallbacks =
    new GoogleApiClient.ConnectionCallbacks() {
      @Override
      public void onConnected(Bundle bundle) {
        Log.d(TAG, "Connected to " + GPGS + ".");
        connectionCallbacks.onConnected();
      }

      @Override
      public void onConnectionSuspended(int cause) {
        Log.d(TAG, "Disconnected from " + GPGS + ": " + cause + ".");
        connectionCallbacks.onDisconnected();
      }
    };

  public U3DGamesClient(ConnectionCallbacks connectionCallbacks) throws ClassNotFoundException, NoSuchFieldException, IllegalAccessException {
    activity = (Activity) Class.forName("com.unity3d.player.UnityPlayer").getField("currentActivity").get(null);
    this.connectionCallbacks = connectionCallbacks;

    playServicesSupported =
      GooglePlayServicesUtil.isGooglePlayServicesAvailable(activity);

    if (playServicesSupported == ConnectionResult.SUCCESS) {
      GoogleSignInOptions builder = new GoogleSignInOptions.Builder(GoogleSignInOptions.DEFAULT_SIGN_IN).requestEmail().build();
      client = GoogleSignIn.getClient(activity, builder);
    } else {
      client = null;
    }

    id = System.currentTimeMillis();
    StaticData.clients.put(id, this);
  }

  public void connect() {
    Log.d(TAG, "connect()");
    Intent signInIntent = client.getSignInIntent();
    // random id
    activity.startActivityForResult(signInIntent, 7382642);
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
                activity.startActivityForResult(intent, 134234345);
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
                activity.startActivityForResult(intent, 134234345);
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
