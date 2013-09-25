# What is this?

This is Android part of Unity3D - Google Play Game Services bridge.

It is compiled into a .jar which is then put into your Unity3D projects
\Assets\Plugins\Android\ directory for usage.

# Usage

As an end user you should check out main README.md file.

# Building

Want to contribute? Great!

For building you need the following:

* IntelliJ IDEA Community Edition with Android plugin (or Android Studio I
  guess).
* JDK version 6.
* Android SDK with Android 2.3.3 platform installed.

The process looks like this:

1. Create a new android project in this dir.
2. Setup it with JDK6 and Android 2.3.3.
3. Add libraries from **libs** folder as project libraries.
4. Create an artifact with module dependencies extracted. Remove Unity3D player
   from that jar.
5. Use menu item **Build > Build Artifacts > Build**.
6. Copy created .jar to \csharp\Assets\Plugins\Android\

# Support

All support for this project is provided via a
[google group](https://groups.google.com/forum/#!forum/u3d_gps_bridge).