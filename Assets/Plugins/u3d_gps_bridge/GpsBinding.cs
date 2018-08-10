using UnityEngine;

namespace com.tinylabproductions.TLPGame.u3d_gps_bridge {
  public interface IGpsBinding {
    void submitScore(LeaderboardId id, float score);
    void showLeaderboard(LeaderboardId id);
    void submitAchievement(AchievementId id);
    void showAchievements();

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