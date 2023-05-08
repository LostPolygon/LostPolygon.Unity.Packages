using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LostPolygon.Unity.Utility.Editor {
    public partial struct ReflectionWrapper {
        internal class TypeMemberInfo {
            public readonly Dictionary<string, List<MethodInfoExtended>> MethodInfos;
            public readonly Dictionary<string, FieldInfo> FieldInfos;
            public readonly Dictionary<string, PropertyInfo> PropertyInfos;

            public TypeMemberInfo(Type type) {
                const BindingFlags bindingFlags =
                    BindingFlags.Instance |
                    BindingFlags.Static |
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.FlattenHierarchy;
                MethodInfos = MemberInfosToDictionary(type.GetMethods(bindingFlags).Select(m => new MethodInfoExtended(m)).ToArray(), m => m.MethodInfo.Name);
                FieldInfos = MemberInfosToOneDictionary(type.GetFields(bindingFlags), f => f.Name);
                PropertyInfos = MemberInfosToOneDictionary(type.GetProperties(bindingFlags), p => p.Name);
            }

            private static Dictionary<string, List<T>> MemberInfosToDictionary<T>(IEnumerable<T> memberInfos, Func<T, string> nameGetter) {
                Dictionary<string, List<T>> result = new();
                foreach (T memberInfo in memberInfos) {
                    string memberInfoName = nameGetter(memberInfo);
                    if (!result.TryGetValue(memberInfoName, out List<T> nameMemberInfos)) {
                        nameMemberInfos = new List<T>();
                        result.Add(memberInfoName, nameMemberInfos);
                    }

                    nameMemberInfos.Add(memberInfo);
                }

                return result;
            }

            private static Dictionary<string, T> MemberInfosToOneDictionary<T>(IEnumerable<T> memberInfos, Func<T, string> nameGetter) {
                Dictionary<string, T> result = new();
                foreach (T memberInfo in memberInfos) {
                    string memberInfoName = nameGetter(memberInfo);
                    result.Add(memberInfoName, memberInfo);
                }

                return result;
            }

            public class MethodInfoExtended {
                public readonly MethodInfo MethodInfo;
                public readonly ParameterInfo[] ParameterInfos;
                public readonly Type[] ParameterTypes;

                public MethodInfoExtended(MethodInfo methodInfo) {
                    MethodInfo = methodInfo;
                    ParameterInfos = methodInfo.GetParameters();
                    ParameterTypes = ParameterInfos.Select(p => p.ParameterType).ToArray();
                }
            }
        }
    }
}
