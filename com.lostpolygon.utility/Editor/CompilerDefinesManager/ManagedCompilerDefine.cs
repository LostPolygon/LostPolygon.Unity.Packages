namespace LostPolygon.Unity.Utility.Editor {
    public sealed record ManagedCompilerDefine(string Name, string Description) {
        public bool Equals(ManagedCompilerDefine other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }
    }
}
