#if PART_U3D_GPS_BRIDGE
#if UNITY_ANDROID
using System;
using com.tinylabproductions.TLPLib.Concurrent;
using com.tinylabproductions.TLPLib.Data;
using com.tinylabproductions.TLPLib.Functional;
using com.tinylabproductions.TLPLib.Logger;
using UnityEngine;

namespace com.tinylabproductions.TLPGame.u3d_gps_bridge {
  public class GpsBindingAndroid : IGpsBinding {
    public static readonly IGpsBinding instance = new GpsBindingAndroid();
    public Future<Unit> signedIn => onSignIn;

    // When targeted audience includes kids silentSignIn by default must be disabled.
    // It could only be enabled when user takes action which needs GPS. For example to view leaderboards or achievements.
    readonly PrefVal<bool> silentSignInEnabled = PrefVal.player.boolean("silent_sign_in_enabled", false);
    readonly Future<Unit> onSignIn;
    readonly Client client = new Client();

    GpsBindingAndroid() {
      onSignIn = Future.a<Unit>(p => {
        if (Application.platform == RuntimePlatform.Android)
          client.callbacks.OnSignIn += result => ASync.OnMainThread(() => {
            silentSignInEnabled.value = result == ConnectionCallbacks.SignInResult.Success;
            if (result == ConnectionCallbacks.SignInResult.Success) p.tryComplete(F.unit);

            if (Log.d.isDebug()) Log.d.debug($"{nameof(GpsBindingAndroid)} Gps signed in with result {result}");
          });
      });
      if (Application.platform == RuntimePlatform.Android) {
        client.callbacks.OnDisconnected += () => ASync.OnMainThread(() => silentSignInEnabled.value = false);
      }
      if (!client.supported) return;
      if (silentSignInEnabled.value) client.signIn();
    }

    public void submitScore(LeaderboardId id, float score) {
      if (client.signedIn) client.submitScore(id.value, Mathf.FloorToInt(score));
    }

    public void submitAchievement(AchievementId id) {
      if (client.signedIn) client.unlockAchievement(id.value);
    }

    public void showLeaderboard(LeaderboardId id) => userAction(() => client.showLeaderboard(id.value));
    public void showAchievements() => userAction(() => client.showAchievements());

    void userAction(Action act) {
      if (client.signedIn) act();
      else {
        client.signIn();

        void onSignIn(ConnectionCallbacks.SignInResult result) {
          if (result == ConnectionCallbacks.SignInResult.Success) act();
          client.callbacks.OnSignIn -= onSignIn;
        }

        client.callbacks.OnSignIn += onSignIn;
      }
    }
  }
}
#endif
#endif

