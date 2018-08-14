using com.tinylabproductions.TLPLib.Concurrent;
using com.tinylabproductions.TLPLib.Functional;
using com.tinylabproductions.TLPLib.Logger;

namespace com.tinylabproductions.TLPGame.u3d_gps_bridge {
  public class GpsBindingNoOp : IGpsBinding {
    public static readonly IGpsBinding instance = new GpsBindingNoOp();
    public Future<Unit> signedIn => Future<Unit>.unfulfilled;
    
    GpsBindingNoOp() {}

    public void submitScore(LeaderboardId id, float score) {
      if (Log.d.isDebug())
        Log.d.debug($"{nameof(GpsBindingNoOp)} submitting score: {id} {nameof(score)} {score}");
    }

    public void showLeaderboard(LeaderboardId id) {
      if (Log.d.isDebug())
        Log.d.debug($"{nameof(GpsBindingNoOp)} show leaderboard: {id}");
    }

    public void submitAchievement(AchievementId id) {
      if (Log.d.isDebug())
        Log.d.debug($"{nameof(GpsBindingNoOp)} submitting achievement: {id}");
    }

    public void showAchievements() {
      if (Log.d.isDebug()) Log.d.debug($"{nameof(GpsBindingNoOp)} showing achievements");
    }
  }
}