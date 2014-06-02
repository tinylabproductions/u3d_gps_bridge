using System;
using UnityEngine;

namespace com.tinylabproductions.u3d_gps_bridge {
  /**
   * Unity 3D Google Game Play Services (GGPS) Android client.
   **/
  public class Client {
    public enum ServiceStatus {
      Supported, Missing, VersionUpdateRequired, Disabled, Invalid
    }

    public readonly ServiceStatus serviceStatus;
    public readonly ConnectionCallbacks callbacks;

    private readonly AndroidJavaObject client;

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

    public bool connected { get { return onAndroid && client.Call<bool>("isConnected"); } }

    public bool supported { get {
      return serviceStatus == ServiceStatus.Supported;
    } }

    public void connect() { if (onAndroid) client.Call("connect"); }
    public void reconnect() { if (onAndroid) client.Call("reconnect"); }
    public void disconnect() { if (onAndroid) client.Call("disconnect"); }

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
    public bool showAchievements() {
      return onAndroid && client.Call<bool>("showAchievements");
    }

    /**
     * Returns true if activity for showing leaderboard has been started.
     * 
     * Otherwise games client tries to reconnect.
     **/
    public bool showLeaderboard(string leaderboardId) {
      return onAndroid && client.Call<bool>("showLeaderboard", leaderboardId);
    }

    // Is GGPS supported on this device?
    private ServiceStatus getServiceStatus() {
      if (client.Call<bool>("isSupported")) return ServiceStatus.Supported;
      if (client.Call<bool>("isServiceMissing")) return ServiceStatus.Missing;
      if (client.Call<bool>("isServiceVersionUpdateRequired"))
        return ServiceStatus.VersionUpdateRequired;
      if (client.Call<bool>("isServiceDisabled")) return ServiceStatus.Disabled;
      if (client.Call<bool>("isServiceInvalid")) return ServiceStatus.Invalid;

      throw new Exception("Internal library error: unknown GGPS status!");
    }

    private static bool onAndroid 
      { get { return Application.platform == RuntimePlatform.Android; } }
  }
}
