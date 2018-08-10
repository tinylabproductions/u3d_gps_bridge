#if PART_U3D_GPS_BRIDGE
#if UNITY_ANDROID
using System;
using UnityEngine;

namespace com.tinylabproductions.u3d_gps_bridge {
  /**
   * Connection callbacks class.
   * 
   * When Google Play Game Services events occur, these delegates are called.
   * 
   * Be warned, that they might happen on any thread, so if you want to do 
   * something with Unity (for example access PlayerPrefs), you need to do that on
   * a main thread.
   * 
   * In practice you should use a tool like Loom (http://unitygems.com/threads/)
   * like this:
   * 
   *    client.callbacks.OnConnected += onConnection;
   * 
   *    protected void onConnection() {
   *      Debug.Log("Connected to highscores service.");
   *      Loom.QueueOnMainThread(() => {
   *        PlayerPrefs.SetInt("foo", 10);
   *      });
   *    }
   *    
   * Be sure to initialize Loom in one of your scripts, in Awake() or Start() methods:
   * 
   *    Debug.Log(Loom.Current); // Initialize Loom
   **/
  public class ConnectionCallbacks : AndroidJavaProxy {
    public ConnectionCallbacks() : 
    base("com.tinylabproductions.u3d_gps_bridge.ConnectionCallbacks")
    {}

    public enum SignInResult { Success, Fail, Cancel }

    public Action OnNetworkFailure = delegate {};

    // Called when client disconnects from google play services.
    public Action OnDisconnected = delegate {};

    public Action<SignInResult> OnSignIn = delegate {};


    /** Java interface methods **/

    void onNetworkFailure() { OnNetworkFailure.Invoke(); }
    void onDisconnected() { OnDisconnected.Invoke(); }
    void onSignIn() { OnSignIn.Invoke(SignInResult.Success); }
    void onSignInFailed() { OnSignIn.Invoke(SignInResult.Fail); }
    void onSignInCanceled() { OnSignIn.Invoke(SignInResult.Cancel); }
  }
}
#endif
#endif
