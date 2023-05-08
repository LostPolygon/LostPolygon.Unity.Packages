using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace LostPolygon.Unity.Utility.Editor {
    /// <summary>
    /// Utilities for working with build platforms.
    /// </summary>
    public static class BuildPlatformsUtility {
        public static readonly BuildPlatform[] ValidBuildPlatforms;
        public static readonly Dictionary<BuildPlatformId, BuildPlatform> BuildPlatformIdToBuildPlatformMap;
        public static readonly Array ValidBuildPlatformsRaw;

        static BuildPlatformsUtility() {
            Assembly editorAssembly = typeof(NamedBuildTarget).Assembly;

            // Get UnityEditor.BuildPlayerWindow.BuildPlatform[]
            Type buildPlatformsType = editorAssembly.GetType("UnityEditor.Build.BuildPlatforms");

            ValidBuildPlatformsRaw =
                ReflectionWrapper.Wrap(buildPlatformsType)
                    .Property<object>("instance")
                    .GetWrap()
                    .Method<object>("GetValidPlatforms", Type.EmptyTypes)
                    .InvokeWrapped()
                    .Method<Array>("ToArray")
                    .Invoke();

            // Copy data from UnityEditor.BuildPlayerWindow.BuildPlatform to our internal version
            ValidBuildPlatforms = new BuildPlatform[ValidBuildPlatformsRaw.Length];
            for (int i = 0; i < ValidBuildPlatformsRaw.Length; i++) {
                ReflectionWrapper currentBuildPlatform =
                    ReflectionWrapper.Wrap(ValidBuildPlatformsRaw.GetValue(i));

                BuildTargetGroup buildTargetGroup = currentBuildPlatform.Property<BuildTargetGroup>("targetGroup");
                string name = currentBuildPlatform.Property<GUIContent>("title").Get().text;
                string tooltip = currentBuildPlatform.Field<string>("tooltip");
                int subtarget = 0;
                bool hasSubtarget = currentBuildPlatform.UnwrapAsType().FullName == "UnityEditor.Build.BuildPlatformWithSubtarget";
                if (hasSubtarget) {
                    subtarget = currentBuildPlatform.Field<int>("subtarget");
                }

                NamedBuildTarget namedBuildTarget = currentBuildPlatform.Field<NamedBuildTarget>("namedBuildTarget");

                ReflectionWrapper.PropertyHandle<Texture> smallIconPropertyHandle = currentBuildPlatform.Property<Texture>("smallIcon");
                ReflectionWrapper.FieldHandle<Texture> smallIconFieldHandle = currentBuildPlatform.Field<Texture>("smallIcon");
                Texture smallIcon = smallIconPropertyHandle.Valid ? smallIconPropertyHandle.Get() : smallIconFieldHandle.Get();

                ValidBuildPlatforms[i] =
                    new BuildPlatform(
                        buildTargetGroup,
                        subtarget,
                        namedBuildTarget,
                        name,
                        tooltip,
                        smallIcon
                    );
            }

            // Match BuildPlatformId to BuildPlatform
            BuildPlatformIdToBuildPlatformMap =
                ValidBuildPlatforms.ToDictionary(
                    platform => platform.BuildPlatformId,
                    platform => platform
                );
        }

        public static BuildPlatformId GetSelectedBuildPlatformId() {
            NamedBuildTarget namedBuildTarget =
                EditorUserBuildSettingsUtilsInternals.CalculateSelectedNamedBuildTarget();

            BuildPlatformId buildPlatformId = GetBuildPlatformId(namedBuildTarget);
            return buildPlatformId;
        }

        public static BuildPlatform GetActiveBuildPlatform() {
            NamedBuildTarget namedBuildTarget =
                EditorUserBuildSettingsUtilsInternals.CalculateActiveNamedBuildTarget();

            BuildPlatform buildPlatform = GetBuildPlatform(namedBuildTarget);
            return buildPlatform;
        }

        public static BuildPlatformId GetActiveBuildPlatformId() =>
            GetActiveBuildPlatform().BuildPlatformId;

        public static BuildPlatform GetBuildPlatform(BuildPlatformId buildPlatformId) {
            return BuildPlatformIdToBuildPlatformMap[buildPlatformId];
        }

        public static BuildPlatform GetBuildPlatform(NamedBuildTarget namedBuildTarget) {
            return BuildPlatformIdToBuildPlatformMap
                .First(p => p.Value.NamedBuildTarget == namedBuildTarget)
                .Value;
        }

        public static BuildPlatformId GetBuildPlatformId(NamedBuildTarget namedBuildTarget) {
            if (String.IsNullOrEmpty(namedBuildTarget.TargetName))
                return BuildPlatformId.Default;

            return GetBuildPlatform(namedBuildTarget).BuildPlatformId;
        }
    }
}
