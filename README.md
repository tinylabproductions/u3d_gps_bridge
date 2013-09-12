# Unity3D - Google Play Services bridge

This project allows you to easily integrate Google Play Services into your
Unity3D game.

Currently supported features are:

* Google Play Game Services
** Connecting and signing in.
** Submitting scores to leaderboards.
** Showing leaderboards.

You are free to add more features :)

## Installation

1. Download https://github.com/tinylabproductions/u3d_gps_bridge/archive/master.zip.
2. Extract contents of **csharp** directory into your Unity3D game.
3. Use!

## Usage

      // Create new client
      client = new Client();
      
      // Check if Google Play Services are supported on this device.
      var supported = client.supported;
      if (! supported)
        Debug.LogWarning(
          "Play game services client on this device is not available: " +
          client.serviceStatus
        );
      else {
        // Setup callbacks
        client.callbacks.OnConnected += () => {};
        client.callbacks.OnDisconnected += () => {};
        client.callbacks.OnSignIn += () => {};
        client.callbacks.OnSignInFailed += () => {};
        client.callbacks.OnConnectionFailed += errorCode => {};

        // Connect to services.
        if (! client.connected)
          client.connect();
      }

Submitting a score:

      client.submitScore(leaderboardId, score);

Showing a leaderboard:

      if (client.showLeaderboard(leaderboardId)) {
        // Connection was established, activity launched, 
        // should show up soon.
      }
      else {
        // Connection was not established, trying to connect & sign-in.
      }

# Support

All support is provided via 
[google group](https://groups.google.com/forum/#!forum/u3d_gps_bridge).
