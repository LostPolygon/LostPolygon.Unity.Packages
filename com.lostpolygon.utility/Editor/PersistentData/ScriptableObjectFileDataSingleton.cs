using System;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LostPolygon.Unity.Utility.Editor {
    public class ScriptableObjectFileDataSingleton<TData> : PersistentFileDataSingleton<TData> where TData : ScriptableObject {
        protected ScriptableObjectFileDataSingleton(string filePath, bool monitorFileChanges) : base(filePath, monitorFileChanges) {
        }

        public override void Load(bool changeDetected) {
            if (!String.IsNullOrEmpty(FilePath)) {
                InstanceData = InternalEditorUtility.LoadSerializedFileAndForget(FilePath).FirstOrDefault() as TData;
            }

            if (InstanceData != null)
                return;

            CreateNewInstance();
            Save();
        }

        protected override void SaveInstanceData() {
            InternalEditorUtility.SaveToSerializedFileAndForget(
                new Object[] { InstanceData },
                FilePath,
                true
            );
        }

        public override void Delete() {
            base.Delete();

            if (InstanceData != null) {
                Object.DestroyImmediate(InstanceData);
            }

            CreateNewInstance();
        }

        private void CreateNewInstance() {
            InstanceData = ScriptableObject.CreateInstance<TData>();
            InstanceData.hideFlags = HideFlags.HideAndDontSave;
        }
    }
}
