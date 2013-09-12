package com.tinylabproductions.u3d_gps_bridge;

import android.app.Activity;
import android.content.Intent;
import android.content.IntentSender;
import android.os.Bundle;
import android.util.Log;
import android.view.Window;
import com.google.android.gms.common.ConnectionResult;

public class CallbackActivity extends Activity {
  private static final int REQUEST_SIGN_IN = 0;

  private ConnectionResult result;
  private U3DGamesClient client;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    requestWindowFeature(Window.FEATURE_NO_TITLE);
  }

  @Override
  protected void onStart() {
    super.onStart();
    retrieveIntentData();

    try {
      Log.d(U3DGamesClient.TAG, "Starting sign-in activity.");
      result.startResolutionForResult(this, REQUEST_SIGN_IN);
    } catch (IntentSender.SendIntentException e) {
      Log.e(
        U3DGamesClient.TAG,
        "Error while starting sign in to " + U3DGamesClient.GPGS + ": " + e
      );
      finish();
    }
  }

  private void retrieveIntentData() {
    Intent intent = getIntent();
    long key = intent.getLongExtra(StaticData.KEY, 0);
    if (key == 0)
      throw new IllegalStateException("Cannot get key from intent " + intent);

    result = StaticData.results.get(key);
    client = StaticData.clients.get(key);
    Log.d(U3DGamesClient.TAG, String.format(
      "Retrieved data from intent[key: %d, result: %s, client: %s]",
      key, result, client
    ));
    Log.d(U3DGamesClient.TAG, StaticData.asString());
  }

  @Override
  protected void onActivityResult
    (int requestCode, int resultCode, Intent data) {
    super.onActivityResult(requestCode, resultCode, data);
    retrieveIntentData();
    Log.d(
      U3DGamesClient.TAG,
      "onActivityResult(requestCode: " + requestCode +
        ", resultCode:" + resultCode + ", data: " + data
    );

    if (requestCode == REQUEST_SIGN_IN) {
      if (resultCode == Activity.RESULT_OK) {
        Log.d(
          U3DGamesClient.TAG,
          "Signed in to " + U3DGamesClient.GPGS +
            ". Connecting again & calling callback."
        );
        client.connect();
        client.connectionCallbacks.onSignIn();
      } else {
        Log.w(
          U3DGamesClient.TAG,
          "Sign in to " + U3DGamesClient.GPGS + " failed. Calling callback."
        );
        client.connectionCallbacks.onSignInFailed();
      }
    }

    finish();
  }
}
