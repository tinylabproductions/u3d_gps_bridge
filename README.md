# Unity3D - Google Play Services bridge

This project allows you to easily integrate Google Play Services into your
Unity3D game.

Currently supported features are:

* Connecting and signing in.
* Submitting scores to leaderboards.
* Showing leaderboards.

You are free to add more features :)

## Installation

1. Download https://github.com/tinylabproductions/u3d_gps_bridge/archive/master.zip.
2. Extract contents of **Assets** directory into your Unity3D game.
3. Setup & use!

or if you're using msysgit and want files to be hardlinked:

2. Extract whole archive to your_project/vendor/u3d_gps_bridge
3. Run vendor/u3d_gps_bridge/setup/setup.sh from bash.

## Setup

Add your Google Play Game Services application ID to Plugins\Android\AndroidManifest.xml under <application> tag.

	<!-- Needed for GPGS to recognise your game -->
	<meta-data android:name="com.google.android.gms.games.APP_ID"
		android:value="\ 123456789012" />

	<!-- Needed for proper sign-in to GPGS -->
	<activity android:label="@string/app_name" 
		android:name="com.tinylabproductions.u3d_gps_bridge.CallbackActivity" />

Then add google-play-services_lib library project (you can get it from Android SDK manager) to Plugins\Android\

That's it!

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
        // Setup callbacks.
	//
	// !!! Be sure to take a look at
	// com.tinylabproductions.u3d_gps_bridge.ConnectionCallbacks class
	// comments !!!
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
