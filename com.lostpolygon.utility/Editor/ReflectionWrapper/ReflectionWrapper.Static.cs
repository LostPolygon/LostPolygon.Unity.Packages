using System;
using System.Collections.Generic;
using System.Reflection;

namespace LostPolygon.Unity.Utility.Editor {
    public partial struct ReflectionWrapper {
        private static readonly Dictionary<string, Type> TypeNameToType = new();
        private static readonly Dictionary<Type, TypeMemberInfo> TypeMemberInfos = new();

        public static ReflectionWrapper Wrap(object instance) {
            return new ReflectionWrapper(instance);
        }

        public static ReflectionWrapper Wrap<T>() {
            return new ReflectionWrapper(typeof(T));
        }

        public static Type GetTypeFromAnyAssembly(string typeName) {
            if (TypeNameToType.TryGetValue(typeName, out Type type))
                return type;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                type = assembly.GetType(typeName);
                if (type != null)
                    return type;
            }

            return null;
        }

        public static ReflectionWrapper GetWrappedTypeFromAnyAssembly(string typeName) {
            Type type = GetTypeFromAnyAssembly(typeName);
            if (type == null)
                throw new InvalidOperationException($"Type '{typeName}' not found");

            return Wrap(type);
        }
    }
}
