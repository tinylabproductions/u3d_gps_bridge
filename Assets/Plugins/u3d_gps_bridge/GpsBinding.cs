using com.tinylabproductions.TLPLib.Concurrent;
using com.tinylabproductions.TLPLib.Functional;
using UnityEngine;

namespace com.tinylabproductions.TLPGame.u3d_gps_bridge {
  public enum GooglePlayServicesSignInResult : byte { Success, Fail, Cancel }

  public interface IGpsBinding {
    void submitScore(LeaderboardId id, float score);
    void showLeaderboard(LeaderboardId id);
    void submitAchievement(AchievementId id);
    void showAchievements();
    Future<Unit> signedIn { get; }
    Future<Unit> firsTimeTriedToSignIn { get; }
    Future<GooglePlayServicesSignInResult> onSignIn { get; }
  }
  public class GpsBinding {
    public static readonly IGpsBinding instance =
#if PART_U3D_GPS_BRIDGE && UNITY_ANDROID
      Application.platform == RuntimePlatform.Android
        ? GpsBindingAndroid.instance
        : GpsBindingNoOp.instance
#else
      GpsBindingNoOp.instance
#endif
    ;
  }
}