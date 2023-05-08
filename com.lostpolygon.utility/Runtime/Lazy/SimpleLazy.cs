using System;

namespace LostPolygon.Unity.Utility {
    public class SimpleLazy<T> {
        private readonly Func<T> _valueFactory;

        private bool _isCreated;
        private T _value;

        public T Value => GetValue();

        public SimpleLazy(Func<T> valueFactory) {
            _valueFactory = valueFactory;
        }

        private T GetValue() {
            if (_isCreated)
                return _value;

            _value = _valueFactory();
            _isCreated = true;

            return _value;
        }
    }
}
