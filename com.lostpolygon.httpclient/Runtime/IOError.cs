namespace LostPolygon.Unity.HttpClient {
    public readonly struct IOError<T> {
        public IOError(T value) {
            Value = value;
        }

        public T Value { get; }

        public override string ToString() {
            return Value?.ToString();
        }
    }
}
