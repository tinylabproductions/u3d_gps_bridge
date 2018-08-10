using System;
using System.Collections.Generic;
using com.tinylabproductions.TLPLib.Data;
using com.tinylabproductions.TLPLib.Extensions;
using com.tinylabproductions.TLPLib.unity_serialization;
using GenerationAttributes;
using JetBrains.Annotations;
using UnityEngine;

namespace com.tinylabproductions.TLPGame.u3d_gps_bridge {
  public partial class LeaderboardsAchievementsBinding : MonoBehaviour {
    #region Unity Serialized Fields

#pragma warning disable 649
    // ReSharper disable NotNullMemberIsNotInitialized, FieldCanBeMadeReadOnly.Local, ConvertToConstant.Local
    [SerializeField, NotNull, PublicAccessor] UnityOptionImage _maxCoinsOpt;
    [SerializeField, NotNull, PublicAccessor] UnityOptionImage _totalCoinsOverLifetimeOpt;
    [SerializeField, NotNull, PublicAccessor] UnityOptionImage _achievementsOpt;
    // ReSharper restore NotNullMemberIsNotInitialized, FieldCanBeMadeReadOnly.Local, ConvertToConstant.Local
#pragma warning restore 649

    #endregion

    public IEnumerable<ErrorMsg> onValidate(Achievements achievements, Leaderboards leaderboards) {
      ErrorMsg error(string objectName) => new ErrorMsg($"{objectName} has no visual representation");

      if (!achievements.worldsCompleted.isEmpty() && _achievementsOpt.isNone)
        yield return error(nameof(achievements));

      if (leaderboards.maxCoinsId.isSome && _maxCoinsOpt.isNone)
        yield return error(nameof(leaderboards.maxCoinsId));

      if (leaderboards.totalCoinsOverLifetimeId.isSome && _totalCoinsOverLifetimeOpt.isNone)
        yield return error(nameof(leaderboards.totalCoinsOverLifetimeId));
    }
  }

  [Serializable] public class LeaderboardsAchievementsBindingOption : UnityOption<LeaderboardsAchievementsBinding> { }
}