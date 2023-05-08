using System;

namespace LostPolygon.Unity.BuildMetadata{
    public interface IBasicBuildMetaData {
        string Version { get; }
        DateTime BuildDateTime { get; }
        string GitBranchName { get; }
        string GitCommitHash { get; }
        bool IsReleaseBuild { get; }
    }
}
