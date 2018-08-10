using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AdvancedInspector;
using Assets.Code;
using com.tinylabproductions.TLPLib.Data;
using com.tinylabproductions.TLPLib.Data.typeclasses;
using com.tinylabproductions.TLPLib.unity_serialization;
using GenerationAttributes;
using JetBrains.Annotations;
using UnityEngine;

namespace com.tinylabproductions.TLPGame.u3d_gps_bridge {
  [Serializable] public partial struct AchievementId : IStr  {
    #region Unity Serialized Fields

#pragma warning disable 649
    // ReSharper disable NotNullMemberIsNotInitialized, FieldCanBeMadeReadOnly.Local, ConvertToConstant.Local
    [SerializeField, NotNull, PublicAccessor] string _value;
    // ReSharper restore NotNullMemberIsNotInitialized, FieldCanBeMadeReadOnly.Local, ConvertToConstant.Local
#pragma warning restore 649

    #endregion

    public override string ToString() => asString();
    public string asString() => $"{nameof(AchievementId)}:{_value}";
  }

  [Serializable] public partial struct LeaderboardId : IStr {
    #region Unity Serialized Fields

#pragma warning disable 649
    // ReSharper disable NotNullMemberIsNotInitialized, FieldCanBeMadeReadOnly.Local, ConvertToConstant.Local
    [SerializeField, NotNull, PublicAccessor] string _value;
    // ReSharper restore NotNullMemberIsNotInitialized, FieldCanBeMadeReadOnly.Local, ConvertToConstant.Local
#pragma warning restore 649

    #endregion

    public override string ToString() => asString();
    public string asString() => $"{nameof(LeaderboardId)}:{_value}";
  }

  [Serializable] public class LeaderboardIdOption : UnityOption<LeaderboardId> { }

  [Serializable]
  public partial struct WorldAchievement {
    #region Unity Serialized Fields

#pragma warning disable 649
    // ReSharper disable NotNullMemberIsNotInitialized, FieldCanBeMadeReadOnly.Local, ConvertToConstant.Local
    [SerializeField, NotNull, PublicAccessor] WorldBinding _world;
    [SerializeField, PublicAccessor] AchievementId _gpsId;
    // ReSharper restore NotNullMemberIsNotInitialized, FieldCanBeMadeReadOnly.Local, ConvertToConstant.Local
#pragma warning restore 649

    #endregion
  }

  [Serializable] public partial struct Achievements {
    #region Unity Serialized Fields

#pragma warning disable 649
    // ReSharper disable NotNullMemberIsNotInitialized, FieldCanBeMadeReadOnly.Local, ConvertToConstant.Local
    [SerializeField, NotNull, PublicAccessor] WorldAchievement[] _worldsCompleted;
    // ReSharper restore NotNullMemberIsNotInitialized, FieldCanBeMadeReadOnly.Local, ConvertToConstant.Local
#pragma warning restore 649

    #endregion

    public IEnumerable<ErrorMsg> onValidate(ImmutableList<WorldBinding> worldBindings) {
      var errors =  new List<ErrorMsg>();

      errors.AddRange(
        worldsCompleted
          .Where(achievement => !worldBindings.Contains(achievement.world))
          .Select(achievement => new ErrorMsg($"{nameof(worldBindings)} does not contain {achievement}"))
      );

      if (
        worldsCompleted
          .Select(_ => _.world)
          .Distinct(WorldBinding.worldBindingComparer)
          .Count() != worldsCompleted.Length
      )
        errors.Add(new ErrorMsg($"Worlds in {nameof(worldsCompleted)} are not unique"));

      if (worldsCompleted.Select(_ => _.gpsId).Distinct().Count() != worldsCompleted.Length)
        errors.Add(new ErrorMsg($"Ids in {nameof(worldsCompleted)} are not unique"));

      return errors;
    }
  }

  [Serializable] public partial struct Leaderboards {
    #region Unity Serialized Fields

#pragma warning disable 649
    // ReSharper disable NotNullMemberIsNotInitialized, FieldCanBeMadeReadOnly.Local, ConvertToConstant.Local
    [
      SerializeField, NotNull, PublicAccessor,
      Help(HelpType.Info, "Maximum amount of coins which player had or have to spend.")
    ] LeaderboardIdOption _maxCoinsId;

    [
      SerializeField, NotNull, PublicAccessor,
      Help(HelpType.Info, "Coins accumulated over lifetime. If player spends his coins we do not decrease this number.")
    ] LeaderboardIdOption _totalCoinsOverLifetimeId;
    // ReSharper restore NotNullMemberIsNotInitialized, FieldCanBeMadeReadOnly.Local, ConvertToConstant.Local
#pragma warning restore 649

    #endregion
  }

}