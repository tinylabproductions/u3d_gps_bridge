package com.tinylabproductions.u3d_gps_bridge;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GooglePlayServicesClient;
import com.google.android.gms.common.GooglePlayServicesUtil;
import com.google.android.gms.games.GamesClient;
import com.unity3d.player.UnityPlayer;

public class U3DGamesClient {
  public static final String TAG = "U3DGamesClient";
  public static final String GPGS = "Google Play Game Services";

  // An arbitrary integer that you define as the request code.
  private static final int REQUEST_LEADERBOARD = 0;

  private final int playServicesSupported;
  private final GamesClient client;
  private final Activity activity;
  public final ConnectionCallbacks connectionCallbacks;
  private final long id;

  private final GooglePlayServicesClient.ConnectionCallbacks
    gpscCallbacks = new GooglePlayServicesClient.ConnectionCallbacks() {
    @Override
    public void onConnected(Bundle bundle) {
      Log.d(TAG, "Connected to " + GPGS + ".");
      connectionCallbacks.onConnected();
    }

    @Override
    public void onDisconnected() {
      Log.d(TAG, "Disconnected from " + GPGS + ".");
      connectionCallbacks.onDisconnected();
    }
  };

  private final GooglePlayServicesClient.OnConnectionFailedListener
    connectionFailedListener =
    new GooglePlayServicesClient.OnConnectionFailedListener() {
      @Override
      public void onConnectionFailed(final ConnectionResult connectionResult) {
        switch (connectionResult.getErrorCode()) {
          case ConnectionResult.NETWORK_ERROR:
            Log.i(TAG, "Network error to " + GPGS + ". Reconnecting.");
            client.reconnect();
            break;
          case ConnectionResult.SIGN_IN_REQUIRED:
            Log.d(TAG, "Not signed in to " + GPGS + ". Signing in.");

            StaticData.results.put(id, connectionResult);

            Intent intent = new Intent(activity, CallbackActivity.class);
            intent.addFlags(Intent.FLAG_ACTIVITY_NO_ANIMATION);
            intent.putExtra(StaticData.KEY, id);

            Log.d(TAG, "Invoking Intent: " + intent);
            activity.startActivity(intent);
            break;
          default:
            Log.w(
              TAG, "Connection to Google Play Game Services failed. Reason: " +
              connectionResult
            );
            connectionCallbacks.
              onConnectionFailed(connectionResult.getErrorCode());
        }
      }
    };

  public U3DGamesClient(ConnectionCallbacks connectionCallbacks) {
    activity = UnityPlayer.currentActivity;
    this.connectionCallbacks = connectionCallbacks;

    playServicesSupported =
      GooglePlayServicesUtil.isGooglePlayServicesAvailable(activity);

    if (playServicesSupported == ConnectionResult.SUCCESS) {
      GamesClient.Builder builder = new GamesClient.Builder(
        activity, gpscCallbacks, connectionFailedListener
      );
      client = builder.create();
    } else {
      client = null;
    }

    id = System.currentTimeMillis();
    StaticData.clients.put(id, this);
  }

  public void connect() {
    Log.d(TAG, "connect()");
    client.connect();
  }

  public void reconnect() {
    Log.d(TAG, "reconnect()");
    client.reconnect();
  }

  public void disconnect() {
    Log.d(TAG, "disconnect()");
    client.disconnect();
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
    return client != null && client.isConnected();
  }

  public void submitScore(String leaderboardId, long score) {
    Log.d(TAG, String.format(
      "Submitting score %d to leaderboard %s", score, leaderboardId
    ));
    assertConnectivity();
    client.submitScore(leaderboardId, score);
  }

  public boolean showLeaderboard(String leaderboardId) {
    if (!isConnected()) {
      Log.i(TAG, String.format(
        "Cannot show leaderboard %s, because %s is not connected.",
        leaderboardId, GPGS
      ));

      if (client.isConnecting())
        Log.d(TAG, "Client is currently connecting, not issuing connect.");
      else
        connect();

      return false;
    }

    Log.d(TAG, "Starting activity to show leaderboard " + leaderboardId);
    activity.startActivityForResult(
      client.getLeaderboardIntent(leaderboardId),
      REQUEST_LEADERBOARD
    );
    return true;
  }

  private void assertConnectivity() {
    if (!isConnected())
      throw new IllegalStateException(
        "You need to be connected to perform this operation!"
      );
  }
}
