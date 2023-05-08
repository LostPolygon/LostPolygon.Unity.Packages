using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FieldInfo = System.Reflection.FieldInfo;

namespace LostPolygon.Unity.Utility.Editor {
    public partial struct ReflectionWrapper : IReflectionWrapper {
        private object _instance;
        private Type _type;
        private TypeMemberInfo _typeMemberInfo;
        private bool _initialized;

        public ReflectionWrapper(object instance) {
            _instance = instance;
            _type = null;
            _typeMemberInfo = null;
            _initialized = false;
        }

        public MethodHandle<T> Method<T>(string methodName, Type[] parameterTypes = null) {
            Initialize();
            _typeMemberInfo.MethodInfos.TryGetValue(methodName, out List<TypeMemberInfo.MethodInfoExtended> methodInfos);

            TypeMemberInfo.MethodInfoExtended matchingMethodInfo = null;
            if (parameterTypes == null || methodInfos == null) {
                matchingMethodInfo = methodInfos?[0];
            } else {
                foreach (TypeMemberInfo.MethodInfoExtended methodInfo in methodInfos) {
                    if (parameterTypes.SequenceEqual(methodInfo.ParameterTypes)) {
                        matchingMethodInfo = methodInfo;
                        break;
                    }
                }
            }

            return new MethodHandle<T>(_type, _instance, methodName, matchingMethodInfo?.MethodInfo);
        }

        public MethodHandle<object> Method(string methodName, Type[] parameterTypes = null) {
            return Method<object>(methodName, parameterTypes);
        }

        public FieldHandle<T> Field<T>(string fieldName) {
            Initialize();
            _typeMemberInfo.FieldInfos.TryGetValue(fieldName, out FieldInfo fieldInfo);
            return new FieldHandle<T>(_type, _instance, fieldName, fieldInfo);
        }

        public FieldHandle<object> Field(string fieldName) {
            return Field<object>(fieldName);
        }

        public PropertyHandle<object> Property(string propertyName) {
            return Property<object>(propertyName);
        }

        public PropertyHandle<T> Property<T>(string propertyName) {
            Initialize();
            _typeMemberInfo.PropertyInfos.TryGetValue(propertyName, out PropertyInfo propertyInfo);
            return new PropertyHandle<T>(_type, _instance, propertyName, propertyInfo);
        }

        public object UnwrapAsObject() {
            if (_instance == null && _type != null)
                throw new InvalidOperationException("_instance == null && _type != null");

            return _instance;
        }

        public Type UnwrapAsType() {
            if (_type == null && _instance == null)
                throw new InvalidOperationException("_type == null && _instance == null");

            return _type ?? _instance.GetType();
        }

        public override string ToString() {
            return
                _type != null ?
                    $"Type: {_type}" :
                    $"Instance: {_instance}";
        }

        private void Initialize() {
            if (_initialized)
                return;

            UnpackTypeAndInstance(ref _instance, out _type);
            _typeMemberInfo = GetTypeMemberInfos(_type);

            _initialized = true;
        }

        private static TypeMemberInfo GetTypeMemberInfos(Type type) {
            if (!TypeMemberInfos.TryGetValue(type, out TypeMemberInfo typeMemberInfo)) {
                typeMemberInfo = new TypeMemberInfo(type);
                TypeMemberInfos.Add(type, typeMemberInfo);
            }

            return typeMemberInfo;
        }

        private static void UnpackTypeAndInstance(ref object instance, out Type type) {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            type = instance as Type;
            if (type != null) {
                instance = null;
            } else {
                type = instance.GetType();
            }
        }
    }
}
