using System;

namespace LostPolygon.Unity.Utility {
    public class AssetPathAttribute : Attribute {
        public string ResourcesPath { get; init; }
        public string EditorPath { get; init; }
    }
}
