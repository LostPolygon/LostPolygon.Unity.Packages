using System;
using System.Reflection;

namespace LostPolygon.Unity.Utility.Editor {
    public partial struct ReflectionWrapper {
        public readonly struct MethodHandle<T> : IMemberHandle<MethodInfo> {
            private readonly Type _type;
            private readonly object _instance;
            private readonly string _methodName;
            private readonly MethodInfo _methodInfo;

            public MethodHandle(Type type, object obj, string methodName, MethodInfo methodInfo) {
                _type = type;
                _instance = obj;
                _methodName = methodName;
                _methodInfo = methodInfo;
            }

            public ReflectionWrapper InvokeWrapped(params object[] parameters) {
                return new ReflectionWrapper(Invoke(parameters));
            }

            public T Invoke(params object[] parameters) {
                Validate();
                return (T) _methodInfo.Invoke(_instance, parameters);
            }

            public bool Valid => _methodInfo != null;

            public MethodInfo MemberInfo => MethodInfo;

            public MethodInfo MethodInfo {
                get {
                    Validate();
                    return _methodInfo;
                }
            }

            private void Validate() {
                if (!Valid)
                    throw new InvalidOperationException($"Method '{_methodName}' not found in type {_type.FullName}");
            }
        }
    }
}
