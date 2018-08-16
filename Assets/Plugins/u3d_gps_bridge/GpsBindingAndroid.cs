#if PART_U3D_GPS_BRIDGE
#if UNITY_ANDROID
using System;
using com.tinylabproductions.TLPLib.Concurrent;
using com.tinylabproductions.TLPLib.Data;
using com.tinylabproductions.TLPLib.Functional;
using com.tinylabproductions.TLPLib.Logger;
using com.tinylabproductions.TLPLib.Reactive;
using UnityEngine;

namespace com.tinylabproductions.TLPGame.u3d_gps_bridge {
  public class GpsBindingAndroid : IGpsBinding {
    public static readonly IGpsBinding instance = new GpsBindingAndroid();

    public Future<Unit> signedIn =>
      _onSignIn.filter(result => result == GooglePlayServicesSignInResult.Success).map(_ => F.unit);

    public Future<Unit> firsTimeTriedToSignIn =>
      timesTriedToSignIn.filter(value => value == 1u).toFuture().map(_ => F.unit);

    public Future<GooglePlayServicesSignInResult> onSignIn => _onSignIn;

    // When targeted audience includes kids silentSignIn by default must be disabled.
    // It could only be enabled when user takes action which needs GPS. For example to view leaderboards or achievements.
    readonly PrefVal<bool> silentSignInEnabled = PrefVal.player.boolean("silent_sign_in_enabled", false);
    readonly PrefVal<uint> timesTriedToSignIn = PrefVal.player.uinteger("google_play_services_times_tried_sign_in", 0u);
    readonly Future<GooglePlayServicesSignInResult> _onSignIn;
    readonly Client client = new Client();

    GpsBindingAndroid() {
      _onSignIn = Future.a<GooglePlayServicesSignInResult>(p => {
        if (Application.platform == RuntimePlatform.Android)
          client.callbacks.OnSignIn += result => ASync.OnMainThread(() => {
            silentSignInEnabled.value = result == GooglePlayServicesSignInResult.Success;
            p.tryComplete(result);

            if (Log.d.isDebug()) Log.d.debug($"{nameof(GpsBindingAndroid)} Gps signed in with result {result}");
          });
      });
      if (Application.platform == RuntimePlatform.Android) {
        client.callbacks.OnDisconnected += () => ASync.OnMainThread(() => silentSignInEnabled.value = false);
      }
      if (!client.supported) return;
      if (silentSignInEnabled.value) signIn();
    }

    void signIn() {
      timesTriedToSignIn.value++;
      client.signIn();
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
        signIn();

        void onSignIn(GooglePlayServicesSignInResult result) {
          if (result == GooglePlayServicesSignInResult.Success) act();
          client.callbacks.OnSignIn -= onSignIn;
        }

        client.callbacks.OnSignIn += onSignIn;
      }
    }
  }
}
#endif
#endif

