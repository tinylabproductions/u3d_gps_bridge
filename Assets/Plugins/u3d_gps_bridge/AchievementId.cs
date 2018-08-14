using System;
using com.tinylabproductions.TLPLib.Data.typeclasses;
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
}