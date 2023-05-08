using System;
using System.Reflection;

namespace LostPolygon.Unity.Utility.Editor {
    public partial struct ReflectionWrapper {
        public readonly struct FieldHandle<T> : IMemberHandle<FieldInfo>, IReflectionWrapper {
            private readonly Type _type;
            private readonly object _instance;
            private readonly string _fieldName;
            private readonly FieldInfo _fieldInfo;

            public FieldHandle(Type type, object instance, string fieldName, FieldInfo fieldInfo) {
                _type = type;
                _instance = instance;
                _fieldName = fieldName;
                _fieldInfo = fieldInfo;
            }

            public ReflectionWrapper GetWrapped() {
                return new ReflectionWrapper(Get());
            }

            public T Get() {
                Validate();
                return (T) _fieldInfo.GetValue(_instance);
            }

            public void Set(T value) {
                Validate();
                _fieldInfo.SetValue(_instance, value);
            }

            public bool Valid => _fieldInfo != null;

            public FieldInfo MemberInfo => FieldInfo;

            public FieldInfo FieldInfo {
                get {
                    Validate();
                    return _fieldInfo;
                }
            }

            private void Validate() {
                if (!Valid)
                    throw new InvalidOperationException($"Field '{_fieldName}' not found in type {_type.FullName}");
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

            public static implicit operator T(FieldHandle<T> field) {
                return field.Get();
            }
        }
    }
}
