#if PART_U3D_GPS_BRIDGE
#if UNITY_ANDROID
using System;
using UnityEngine;

namespace com.tinylabproductions.TLPGame.Plugins.u3d_gps_bridge {
  /**
   * Unity 3D Google Game Play Services (GGPS) Android client.
   **/
  public class Client {
    public enum ServiceStatus : byte {
      Supported, Missing, VersionUpdateRequired, Disabled, Invalid
    }

    public readonly ServiceStatus serviceStatus;
    public readonly ConnectionCallbacks callbacks;
    readonly AndroidJavaObject client;

    public Client() {
      if (onAndroid) {
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

    public bool connected => onAndroid && client.Call<bool>("isConnected");
    public bool supported => serviceStatus == ServiceStatus.Supported;

    public void connect() { if (onAndroid) client.Call("connect"); }

    public void submitScore(string leaderboardId, long value) {
      if (onAndroid) 
        client.Call("submitScore", leaderboardId, value);
    }

    /** 
     * Unlocks achievement asynchronously. If there's no network connectivity, 
     * game services tries to resubmit this achievement later. You can treat this
     * operation as always successful.
     **/
    public void unlockAchievement(string achievementId) {
      if (onAndroid) 
        client.Call("unlockAchievement", achievementId);
    }

    /**
     * Returns true if activity for showing achievement has been started.
     * 
     * Otherwise games client tries to reconnect.
     **/
    public bool showAchievements() => onAndroid && client.Call<bool>("showAchievements");

    /**
     * Returns true if activity for showing leaderboard has been started.
     * 
     * Otherwise games client tries to reconnect.
     **/
    public bool showLeaderboard(string leaderboardId) =>
      onAndroid && client.Call<bool>("showLeaderboard", leaderboardId);

    // Is GGPS supported on this device?
    ServiceStatus getServiceStatus() {
      if (client.Call<bool>("isSupported")) return ServiceStatus.Supported;
      if (client.Call<bool>("isServiceMissing")) return ServiceStatus.Missing;
      if (client.Call<bool>("isServiceVersionUpdateRequired")) return ServiceStatus.VersionUpdateRequired;
      if (client.Call<bool>("isServiceDisabled")) return ServiceStatus.Disabled;
      if (client.Call<bool>("isServiceInvalid")) return ServiceStatus.Invalid;

      throw new Exception("Internal library error: unknown GGPS status!");
    }

    static bool onAndroid => Application.platform == RuntimePlatform.Android;
  }
}
#endif
#endif
