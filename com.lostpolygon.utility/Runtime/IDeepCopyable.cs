namespace LostPolygon.Unity.Utility {
    public interface IDeepCopyable<T> where T : new() {
        void DeepCopyTo(T destination);

        public T DeepClone() {
            T clone = new();
            DeepCopyTo(clone);

            return clone;
        }
    }
}
