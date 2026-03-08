using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace LostPolygon.Unity.Utility.Editor {
    public class CompilerDefinesManagerWindow : EditorWindow {
        private List<(ManagedCompilerDefine defineDefinition, bool enabled)> _defines;

        private void OnEnable() {
            titleContent = new GUIContent("Compiler Flags Manager");
            
            ResetDefinesList(NamedBuildTarget);
        }

        private void ResetDefinesList(NamedBuildTarget namedBuildTarget) {
            _defines =
                CompilerDefinesManager.CompilerDefines
                    .Select(definition => (
                        definition,
                        CompilerDefinesManager
                            .GetManagedDefines(namedBuildTarget)
                            .Contains(definition.Name)
                    ))
                    .ToList();
        }

        private void OnGUI() {
            GUILayout.Space(10);
            GUILayout.Label($"  Build Target: {NamedBuildTarget.TargetName}", EditorStyles.boldLabel);
            GUILayout.Space(10);

            GUI.enabled = !EditorApplication.isCompiling;

            for (int i = 0; i < _defines.Count; i++) {
                (ManagedCompilerDefine defineDefinition, bool enabled) = _defines[i];

                GUILayout.BeginHorizontal();
                {
                    enabled = GUILayout.Toggle(enabled, defineDefinition.Name, GUI.skin.button, GUILayout.Width(270));

                    GUIStyle labelWordWrap = new GUIStyle(EditorStyles.label) {
                        wordWrap = true
                    };
                    EditorGUILayout.LabelField(defineDefinition.Description, labelWordWrap);
                }
                GUILayout.EndHorizontal();

                _defines[i] = (defineDefinition, enabled);
            }

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Reset", GUILayout.Width(120))) {
                    ResetDefinesList(NamedBuildTarget);
                }

                if (GUILayout.Button("Apply", GUILayout.Width(120))) {
                    CompilerDefinesManager.SetManagedDefines(
                        NamedBuildTarget,
                        _defines
                            .Where(d => d.enabled)
                            .Select(d => d.defineDefinition.Name)
                            .ToArray()
                    );
                    UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
                }
            }
            GUILayout.EndHorizontal();
        }

        public static void OpenWindow() {
            CreateWindow<CompilerDefinesManagerWindow>();
        }
        
        private static NamedBuildTarget NamedBuildTarget {
            get {
                BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
                StandaloneBuildSubtarget subtarget = EditorUserBuildSettings.standaloneBuildSubtarget;
                BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
                return 
                    buildTargetGroup == BuildTargetGroup.Standalone && subtarget == StandaloneBuildSubtarget.Server ? 
                        NamedBuildTarget.Server : 
                        NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            }
        }
    }
}
