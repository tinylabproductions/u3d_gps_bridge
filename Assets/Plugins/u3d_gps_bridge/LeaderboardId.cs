using System;
using com.tinylabproductions.TLPLib.Data.typeclasses;
using com.tinylabproductions.TLPLib.unity_serialization;
using GenerationAttributes;
using JetBrains.Annotations;
using UnityEngine;

namespace com.tinylabproductions.TLPGame.Plugins.u3d_gps_bridge {
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
}