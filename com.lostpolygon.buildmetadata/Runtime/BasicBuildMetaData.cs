using System;

namespace LostPolygon.Unity.BuildMetadata{
    public record BasicBuildMetaData(
        string Version,
        DateTime BuildDateTime,
        string GitBranchName,
        string GitCommitHash,
        bool IsReleaseBuild
    ) : IBasicBuildMetaData;
}
