using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;

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
        
        public static List<string> GetManagedDefines(BuildTargetGroup buildTargetGroup) {
            return GetManagedDefines(NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup));
        }

        public static void SetManagedDefines(BuildTargetGroup buildTargetGroup, IReadOnlyList<string> defines) {
            SetManagedDefines(NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup), defines);
        }

        public static List<string> GetManagedDefines(NamedBuildTarget namedBuildTarget) {
            return GetDefinesForBuildTargetGroup(namedBuildTarget)
                .Where(define => CompilerDefines.Any(def => def.Name == define))
                .ToList();
        }

        public static void SetManagedDefines(NamedBuildTarget namedBuildTarget, IReadOnlyList<string> defines) {
            List<string> list = GetDefinesForBuildTargetGroup(namedBuildTarget)
                .Where(define => CompilerDefines.All(def => def.Name != define))
                .Concat(defines)
                .ToList();

            SetDefinesForBuildTargetGroup(namedBuildTarget, list);
        }

        private static string[] GetDefinesForBuildTargetGroup(NamedBuildTarget namedBuildTarget) {
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out string[] defines);
                return defines;
        }

        private static void SetDefinesForBuildTargetGroup(NamedBuildTarget namedBuildTarget, IReadOnlyList<string> defines) {
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defines.ToArray());
        }
    }
}
