using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LostPolygon.Unity.Utility.Editor {
    /// <summary>
    /// Allows to easily extend upon existing Editor without actually inheriting it.
    /// </summary>
    public abstract class EditorDecorator : UnityEditor.Editor {
        [SerializeField]
        private UnityEditor.Editor _decoratedEditor;

        protected abstract Type DecoratedEditorType { get; }

        protected UnityEditor.Editor DecoratedEditor => _decoratedEditor;

        #region Overrides

        public override void OnInspectorGUI() {
            EnsureDecoratedEditor();
            CallDecoratedOnInspectorGUI();
        }

        public override bool HasPreviewGUI() {
            UpdateDecoratedEditorState();
            return _decoratedEditor.HasPreviewGUI();
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background) {
            UpdateDecoratedEditorState();
            _decoratedEditor.OnPreviewGUI(rect, background);
        }

        public override string GetInfoString() {
            return _decoratedEditor.GetInfoString();
        }

        public override bool RequiresConstantRepaint() {
            EnsureDecoratedEditor();
            return _decoratedEditor.RequiresConstantRepaint();
        }

        public override GUIContent GetPreviewTitle() {
            return _decoratedEditor.GetPreviewTitle();
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {
            return _decoratedEditor.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        public override void OnPreviewSettings() {
            _decoratedEditor.OnPreviewSettings();
        }

        public override void ReloadPreviewInstances() {
            _decoratedEditor.ReloadPreviewInstances();
        }

        public override bool UseDefaultMargins() {
            return _decoratedEditor.UseDefaultMargins();
        }

        public override string ToString() {
            return _decoratedEditor.ToString();
        }

        protected override void OnHeaderGUI() {
            UpdateDecoratedEditorState();

            ReflectionWrapper.Wrap(_decoratedEditor).Method(nameof(OnHeaderGUI)).Invoke();
        }

        #endregion

        #region Unity methods

        protected virtual void OnEnable() {
            CreateDecoratedInspector();
        }

        protected virtual void OnDisable() {
            DestroyDecoratedInspector();
        }

        #endregion

        #region Decorated object manipulation

        protected void EnsureDecoratedEditor() {
            if (_decoratedEditor != null)
                return;

            CreateDecoratedInspector();
            if (_decoratedEditor != null)
                return;

            Debug.LogError("_decoratedEditor == null");
        }

        protected void CallDecoratedOnInspectorGUI() {
            EnsureDecoratedEditor();

            _decoratedEditor.OnInspectorGUI();
        }

        protected virtual void UpdateDecoratedEditorState() {
            EnsureDecoratedEditor();
            _decoratedEditor.SetReferenceTargetIndex(this.GetReferenceTargetIndex());

            //_decoratedEditor.UpdateReferenceTargetIndex(DecoratedEditorType);
        }

        protected void CallDecoratedOnSceneGUI() {
            ReflectionWrapper.Wrap(_decoratedEditor).Method("OnSceneGUI").Invoke();
        }

        protected void CreateDecoratedInspector() {
            DestroyDecoratedInspector();

            _decoratedEditor =
                CreateEditor(
                    targets.Where(o => o != null).ToArray(),
                    DecoratedEditorType
                );
        }

        protected void DestroyDecoratedInspector() {
            if (_decoratedEditor != null) {
                DestroyImmediate(_decoratedEditor);
            }
        }

        #endregion
    }
}
