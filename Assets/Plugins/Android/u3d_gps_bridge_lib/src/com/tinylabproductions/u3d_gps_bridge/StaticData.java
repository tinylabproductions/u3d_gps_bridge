package com.tinylabproductions.u3d_gps_bridge;

import com.google.android.gms.common.ConnectionResult;

import java.util.HashMap;
import java.util.Map;

// Class for passing data between activities.
public class StaticData {
  public static final String KEY = "KEY";

  public static Map<Long, ConnectionResult> results =
    new HashMap<Long, ConnectionResult>();
  public static Map<Long, U3DGamesClient> clients =
    new HashMap<Long, U3DGamesClient>();

  public static String asString() {
    return String.format(
      "StaticData[results: %s, clients: %s]",
      results.toString(), clients.toString()
    );
  }
}
