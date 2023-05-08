using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LostPolygon.Unity.Utility.Editor {
    public class CompilerDefinesManagerWindow : EditorWindow {
        private List<(ManagedCompilerDefine defineDefinition, bool enabled)> _defines;

        private void OnEnable() {
            titleContent = new GUIContent("Compiler Flags Manager");

            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            ResetDefinesList(buildTargetGroup);
        }

        private void ResetDefinesList(BuildTargetGroup buildTargetGroup) {
            _defines =
                CompilerDefinesManager.CompilerDefines
                    .Select(definition => (
                        definition,
                        CompilerDefinesManager
                            .GetManagedDefinesForBuildTargetGroup(buildTargetGroup)
                            .Contains(definition.Name)
                    ))
                    .ToList();
        }

        private void OnGUI() {
            GUILayout.Space(10);
            GUILayout.Label($"  Build Target: {EditorUserBuildSettings.selectedBuildTargetGroup}", EditorStyles.boldLabel);
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
                    ResetDefinesList(EditorUserBuildSettings.selectedBuildTargetGroup);
                }

                if (GUILayout.Button("Apply", GUILayout.Width(120))) {
                    CompilerDefinesManager.SetManagedDefinesForBuildTargetGroup(
                        EditorUserBuildSettings.selectedBuildTargetGroup,
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
    }
}
