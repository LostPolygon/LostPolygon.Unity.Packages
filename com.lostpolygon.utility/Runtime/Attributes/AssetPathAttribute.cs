using System;

namespace LostPolygon.Unity.Utility {
    public class AssetPathAttribute : Attribute {
        public string ResourcesPath { get; init; }
#if UNITY_EDITOR
        public string EditorPath { get; init; }
#endif
    }
}
