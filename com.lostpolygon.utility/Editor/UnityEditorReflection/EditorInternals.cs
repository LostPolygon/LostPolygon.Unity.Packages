using System.Reflection;

namespace LostPolygon.Unity.Utility.Editor {
    /// <summary>
    /// Exposes non-public members of the <see cref="UnityEditor.Editor"/> via reflection.
    /// </summary>
    public static class EditorInternals {
        private static readonly PropertyInfo ReferenceTargetIndexProperty =
            ReflectionWrapper.Wrap(typeof(UnityEditor.Editor)).Property<int>("referenceTargetIndex").PropertyInfo;

        private static readonly FieldInfo HideInspectorField =
            ReflectionWrapper.Wrap(typeof(UnityEditor.Editor)).Field<bool>("hideInspector").FieldInfo;

        public static int GetReferenceTargetIndex(this UnityEditor.Editor editor) {
            return (int) ReferenceTargetIndexProperty.GetValue(editor, null);
        }

        public static void SetReferenceTargetIndex(this UnityEditor.Editor editor, int referenceTargetIndex) {
            ReferenceTargetIndexProperty.SetValue(editor, referenceTargetIndex, null);
        }

        public static void SetHideInspector(this UnityEditor.Editor editor, bool hide) {
            HideInspectorField.SetValue(editor, hide);
        }

        public static void RepaintAllInspectors() {
            ReflectionWrapper
                .Wrap(typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow"))
                .Method("RepaintAllInspectors")
                .Invoke();
        }
    }
}
