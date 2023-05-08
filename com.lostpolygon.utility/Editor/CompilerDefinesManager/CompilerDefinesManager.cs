using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace LostPolygon.Unity.Utility.Editor {
    public static class CompilerDefinesManager {
        public static readonly IReadOnlyList<ManagedCompilerDefine> CompilerDefines;

        static CompilerDefinesManager() {
            HashSet<ManagedCompilerDefine> defines = new();

            MethodInfo[] definesMethods =
                TypeCache.GetMethodsWithAttribute<ManagedCompilerDefineAttribute>()
                    .Where(m => m.IsStatic)
                    .Where(m => typeof(IEnumerable<ManagedCompilerDefine>).IsAssignableFrom(m.ReturnType))
                    .ToArray();

            foreach (MethodInfo definesMethod in definesMethods) {
                IEnumerable<ManagedCompilerDefine> methodDefines = (IEnumerable<ManagedCompilerDefine>) definesMethod.Invoke(null, null);
                defines.UnionWith(methodDefines);
            }

            CompilerDefines = defines.ToArray();
        }

        public static List<string> GetManagedDefinesForBuildTargetGroup(BuildTargetGroup buildTargetGroup) {
            return GetDefinesForBuildTargetGroup(buildTargetGroup)
                .Where(define => CompilerDefines.Any(def => def.Name == define))
                .ToList();
        }

        public static void SetManagedDefinesForBuildTargetGroup(BuildTargetGroup buildTargetGroup, IReadOnlyList<string> defines) {
            List<string> list = GetDefinesForBuildTargetGroup(buildTargetGroup)
                .Where(define => CompilerDefines.All(def => def.Name != define))
                .Concat(defines)
                .ToList();

            SetDefinesForBuildTargetGroup(buildTargetGroup, list);
        }

        private static List<string> GetDefinesForBuildTargetGroup(BuildTargetGroup buildTargetGroup) {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup)
                .Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }

        private static void SetDefinesForBuildTargetGroup(BuildTargetGroup buildTargetGroup, IReadOnlyList<string> defines) {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines.ToArray());
        }
    }
}
