using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;

namespace LostPolygon.Unity.Utility.Editor {
    public class UnityAdditionalLinkXmlGenerator : IUnityLinkerProcessor {
        public int callbackOrder => 0;

        public string GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data) {
            // Preserve all types mentioned with [Inject] attributes
            static void PreserveZenjectInjects(HashSet<Type> preservedTypes) {
                Type injectAttributeType = Type.GetType("Zenject.InjectAttribute, Zenject-usage");
                if (injectAttributeType == null)
                    return;

                TypeCache.MethodCollection injectMethods = TypeCache.GetMethodsWithAttribute(injectAttributeType);
                TypeCache.FieldInfoCollection injectedFields = TypeCache.GetFieldsWithAttribute(injectAttributeType);
                foreach (FieldInfo injectedField in injectedFields) {
                    preservedTypes.Add(injectedField.FieldType);
                }

                foreach (MethodInfo injectMethod in injectMethods) {
                    preservedTypes.Add(injectMethod.DeclaringType);
                    preservedTypes.UnionWith(injectMethod.GetParameters().Select(p => p.ParameterType));
                }
            }

            TypeCache.TypeCollection preserveAllTypes = TypeCache.GetTypesWithAttribute<PreserveAllAttribute>();

            HashSet<Type> preservedTypes = new HashSet<Type>();
            preservedTypes.UnionWith(preserveAllTypes);

#if LP_NEWTONSOFT_JSON_ENABLED
            TypeCache.FieldInfoCollection jsonPropertyFields = TypeCache.GetFieldsWithAttribute<Newtonsoft.Json.JsonPropertyAttribute>();
            preservedTypes.UnionWith(jsonPropertyFields.Select(f => f.DeclaringType));
#endif

            PreserveZenjectInjects(preservedTypes);

            // Add nested types
            foreach (Type type in preservedTypes.ToArray()) {
                preservedTypes.UnionWith(type.GetNestedTypes());
            }

            preservedTypes.RemoveWhere(t => t.IsGenericParameter || t.IsGenericType);

            // Generate link.xml
            XmlDocument linkXml = new XmlDocument();
            XmlElement linkerElement = linkXml.CreateElement("linker");
            linkXml.AppendChild(linkerElement);

            IEnumerable<IGrouping<Assembly, Type>> typesByAssembly = preservedTypes.GroupBy(type => type.Assembly);
            foreach (IGrouping<Assembly, Type> grouping in typesByAssembly) {
                XmlElement assemblyElement = linkXml.CreateElement("assembly");
                assemblyElement.SetAttribute("fullname", grouping.Key.GetName().Name);
                linkerElement.AppendChild(assemblyElement);

                foreach (Type preservedType in grouping) {
                    XmlElement typeElement = linkXml.CreateElement("type");
                    string typeFullName = preservedType.FullName;

                    // link.xml uses "/" to separate class from nested class instead of "+"
                    typeFullName = typeFullName.Replace("+", "/");

                    typeElement.SetAttribute("fullname", typeFullName);
                    typeElement.SetAttribute("preserve", "all");

                    assemblyElement.AppendChild(typeElement);
                }
            }

            string linkXmlPath = Path.GetTempFileName() + ".xml";
            linkXml.Save(linkXmlPath);
            return linkXmlPath;
        }

        public void OnBeforeRun(BuildReport report, UnityLinkerBuildPipelineData data) {
        }

        public void OnAfterRun(BuildReport report, UnityLinkerBuildPipelineData data) {
        }
    }
}
