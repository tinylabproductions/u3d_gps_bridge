#if PART_U3D_GPS_BRIDGE
#if UNITY_ANDROID
using System;
using UnityEngine;

namespace com.tinylabproductions.TLPGame.u3d_gps_bridge {
  /**
   * Unity 3D Google Game Play Services (GGPS) Android client.
   **/
  public class Client {
    public enum ServiceStatus : byte {
      Supported, Missing, VersionUpdateRequired, Disabled, Invalid
    }

    public readonly ConnectionCallbacks callbacks;
    readonly ServiceStatus serviceStatus;
    readonly AndroidJavaObject client;

    public Client() {
      if (onAndroidPlatform) {
        callbacks = new ConnectionCallbacks();
        client = new AndroidJavaObject(
          "com.tinylabproductions.u3d_gps_bridge.U3DGamesClient",
          callbacks
        );
        serviceStatus = getServiceStatus();
      }
      else {
        callbacks = null;
        client = null;
        serviceStatus = ServiceStatus.Disabled;
      }
    }

    public bool connected => onAndroidPlatform && client.Call<bool>("isConnected");
    public bool supported => serviceStatus == ServiceStatus.Supported;

    public void connect() => onAndroid(() => client.Call("connect"));

    public void submitScore(string leaderboardId, long value) =>
      onAndroid(() => client.Call("submitScore", leaderboardId, value));

    /** 
     * Unlocks achievement asynchronously. If there's no network connectivity, 
     * game services tries to resubmit this achievement later. You can treat this
     * operation as always successful. ??????????????????????????????????????????
     **/
    public void unlockAchievement(string achievementId) =>
      onAndroid(() => client.Call("unlockAchievement", achievementId));

    public void showAchievements() => onAndroid(() => client.Call("showAchievements"));

    public void showLeaderboard(string leaderboardId) =>
      onAndroid(() => client.Call("showLeaderboard", leaderboardId));

    // Is GGPS supported on this device?
    ServiceStatus getServiceStatus() {
      if (client.Call<bool>("isSupported")) return ServiceStatus.Supported;
      if (client.Call<bool>("isServiceMissing")) return ServiceStatus.Missing;
      if (client.Call<bool>("isServiceVersionUpdateRequired")) return ServiceStatus.VersionUpdateRequired;
      if (client.Call<bool>("isServiceDisabled")) return ServiceStatus.Disabled;
      if (client.Call<bool>("isServiceInvalid")) return ServiceStatus.Invalid;

      throw new Exception("Internal library error: unknown GGPS status!");
    }

    static void onAndroid(Action action) { if (onAndroidPlatform) action(); }
    static bool onAndroidPlatform => Application.platform == RuntimePlatform.Android;
  }
}
#endif
#endif
