#if PART_U3D_GPS_BRIDGE
// Serializable stuff should always be enabled
#endif

using System;
using System.Collections.Immutable;
using com.tinylabproductions.TLPGame.BuildTools;
using com.tinylabproductions.TLPGame.BuildTools.Data;
using com.tinylabproductions.TLPGame.BuildTools.Data.interfaces;
using com.tinylabproductions.TLPGame.BuildTools.Gradle;
using com.tinylabproductions.TLPLib.Extensions;
using com.tinylabproductions.TLPLib.Functional;
using com.tinylabproductions.TLPLib.Logger;
using com.tinylabproductions.TLPLib.unity_serialization;
using GenerationAttributes;
using JetBrains.Annotations;
using UnityEngine;

namespace com.tinylabproductions.TLPGame.u3d_gps_bridge {
  [Serializable]
  public class U3dGpsBridgeBuildDataUnityOption : UnityOption<U3dGpsBridgeBuildData> {
    public U3dGpsBridgeBuildDataUnityOption() { }
    public U3dGpsBridgeBuildDataUnityOption(Option<U3dGpsBridgeBuildData> value) : base(value) { }
  }

  [Serializable]
  public class U3dGpsBridgeBuildData : AppPartBuildDataUnit {
    #region Unity Serialized Fields

#pragma warning disable 649
    // ReSharper disable NotNullMemberIsNotInitialized, FieldCanBeMadeReadOnly.Local, ConvertToConstant.Local
    [SerializeField, NotNull] string _clientId;
    // ReSharper restore NotNullMemberIsNotInitialized, FieldCanBeMadeReadOnly.Local, ConvertToConstant.Local
#pragma warning restore 649

    #endregion

    public const AppPartType DATA_FOR = AppPartType.U3dGpsBridge;
    public override AppPartType dataFor => DATA_FOR;

    public override void onValidate(BuildScript buildScript, ILog log) { }

    public override Either<ImmutableList<string>, IAppPartBuildDataValidated> validate(
      AppPartBuildDataValidationArgs args
    ) => new U3dGpsBridgeBuildDataValidated(_clientId);
  }

  [Record]
  public partial class U3dGpsBridgeBuildDataValidated : IAppPartBuildDataValidated,
    IPreGradleifyAddFileContentReplacements
  {
    public readonly string clientId;

    public AppPartType dataFor => U3dGpsBridgeBuildData.DATA_FOR;

    public Option<IPreGradleifyAddFileContentReplacements> preGradleifyAddFileContentReplacements =>
      ((IPreGradleifyAddFileContentReplacements) this).some();

    public Option<IPostGradleifyAction> postGradleifyAction =>
      Option<IPostGradleifyAction>.None;

    public Option<IGradleifyHasMainScriptDependencies> gradleifyHasMainScriptDependencies =>
      Option<IGradleifyHasMainScriptDependencies>.None;

    public Option<IGradleifyHasMainScriptAddition> gradleifyHasMainScriptAddition =>
      Option<IGradleifyHasMainScriptAddition>.None;

    public void modifyReplacements(
      IBuild build, IBuildAndroid android, Builder.GradleFileContentsReplacementsBuilder builder
    ) {
       builder.addManifestReplacement(
         dataFor,
         ImmutableArray.Create(new FileContentsReplace.Replacement("%%GOOGLE-PLAY-SERVICES-ID%%", clientId))
       );
    }
  }
}