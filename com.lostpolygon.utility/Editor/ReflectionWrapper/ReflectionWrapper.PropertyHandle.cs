using System;
using System.Reflection;

namespace LostPolygon.Unity.Utility.Editor {
    public partial struct ReflectionWrapper {
        public readonly struct PropertyHandle<T> : IMemberHandle<PropertyInfo>, IReflectionWrapper {
            private readonly Type _type;
            private readonly object _instance;
            private readonly string _propertyName;
            private readonly PropertyInfo _propertyInfo;

            public PropertyHandle(Type type, object instance, string propertyName, PropertyInfo propertyInfo) {
                _type = type;
                _instance = instance;
                _propertyName = propertyName;
                _propertyInfo = propertyInfo;
            }

            public ReflectionWrapper GetWrap() {
                return new ReflectionWrapper((T) _propertyInfo.GetValue(_instance, null));
            }

            public ReflectionWrapper GetWrapped() {
                return new ReflectionWrapper(Get());
            }

            public T Get() {
                Validate();
                return (T) _propertyInfo.GetValue(_instance, null);
            }

            public void Set(T value) {
                Validate();
                _propertyInfo.SetValue(_instance, value, null);
            }

            public bool Valid => _propertyInfo != null;

            public PropertyInfo MemberInfo => PropertyInfo;

            public PropertyInfo PropertyInfo {
                get {
                    Validate();
                    return _propertyInfo;
                }
            }

            private void Validate() {
                if (!Valid)
                    throw new InvalidOperationException($"Property '{_propertyName}' not found in type {_type.FullName}");
            }

            #region IReflectionWrapper

            public MethodHandle<T1> Method<T1>(string methodName, Type[] parameterTypes = null) {
                ReflectionWrapper reflectionWrapper = GetWrapped();
                return reflectionWrapper.Method<T1>(methodName, parameterTypes);
            }

            public MethodHandle<object> Method(string methodName, Type[] parameterTypes = null) {
                ReflectionWrapper reflectionWrapper = GetWrapped();
                return reflectionWrapper.Method(methodName, parameterTypes);
            }

            public FieldHandle<T1> Field<T1>(string fieldName) {
                ReflectionWrapper reflectionWrapper = GetWrapped();
                return reflectionWrapper.Field<T1>(fieldName);
            }

            public FieldHandle<object> Field(string fieldName) {
                ReflectionWrapper reflectionWrapper = GetWrapped();
                return reflectionWrapper.Field(fieldName);
            }

            public PropertyHandle<object> Property(string propertyName) {
                ReflectionWrapper reflectionWrapper = GetWrapped();
                return reflectionWrapper.Property(propertyName);
            }

            public PropertyHandle<T1> Property<T1>(string propertyName) {
                ReflectionWrapper reflectionWrapper = GetWrapped();
                return reflectionWrapper.Property<T1>(propertyName);
            }

            #endregion

            public static implicit operator T(PropertyHandle<T> prop) {
                return prop.Get();
            }
        }
    }
}
