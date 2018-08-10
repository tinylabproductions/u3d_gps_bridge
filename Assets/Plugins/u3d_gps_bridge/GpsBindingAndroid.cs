#if PART_U3D_GPS_BRIDGE
#if UNITY_ANDROID
using System;
using com.tinylabproductions.TLPLib.Concurrent;
using com.tinylabproductions.TLPLib.Data;
using com.tinylabproductions.TLPLib.Functional;
using com.tinylabproductions.TLPLib.Logger;
using com.tinylabproductions.TLPLib.Reactive;
using UnityEngine;

namespace com.tinylabproductions.TLPGame.Plugins.u3d_gps_bridge {
  public class GpsBindingAndroid : IGpsBinding {
    public static readonly IGpsBinding instance = new GpsBindingAndroid();

    // When targeted audience includes kids silentSignIn by default must be disabled.
    // It could only be enabled when user takes action which needs GPS. For example to view leaderboards or achievements.
    readonly PrefVal<bool> silentSignInEnabled = PrefVal.player.boolean("silent_sign_in_enabled", false);
    public readonly Future<ConnectionCallbacks.SignInResult> onSignIn;
    public readonly Subject<Unit> onNetworkFailure = new Subject<Unit>();
    readonly Client client = new Client();

    GpsBindingAndroid() {
      onSignIn = Future.a<ConnectionCallbacks.SignInResult>(p => {
        if (Application.platform == RuntimePlatform.Android)
          client.callbacks.OnSignIn += result => ASync.OnMainThread(() => {
            silentSignInEnabled.value = result == ConnectionCallbacks.SignInResult.Success;
            p.tryComplete(result);
            if (Log.d.isDebug()) Log.d.debug($"{nameof(GpsBindingAndroid)} Gps signed in with result {result}");
          });
      });
      if (Application.platform == RuntimePlatform.Android) {
        client.callbacks.OnDisconnected += () => ASync.OnMainThread(() => silentSignInEnabled.value = false);
        client.callbacks.OnNetworkFailure += () => ASync.OnMainThread(() => onNetworkFailure.push(F.unit));
      }
      if (!client.supported) return;
      if (silentSignInEnabled.value) client.connect();
    }

    public void submitScore(LeaderboardId id, float score) {
      if (client.connected) client.submitScore(id.value, Mathf.FloorToInt(score));
    }

    public void submitAchievement(AchievementId id) {
      if (client.connected) client.unlockAchievement(id.value);
    }

    public void showLeaderboard(LeaderboardId id) => userAction(() => client.showLeaderboard(id.value));
    public void showAchievements() => userAction(() => client.showAchievements());

    void userAction(Action act) {
      if (client.connected) act();
      else {
        silentSignInEnabled.value = true;
        if (!client.connected) client.connect();

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

