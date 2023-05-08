using System;
using System.IO;
using UnityEngine;

namespace LostPolygon.Unity.Utility.Editor {
    public class UnityJsonFileDataSingleton<TData> : PersistentFileDataSingleton<TData> where TData : class, new() {
        public UnityJsonFileDataSingleton(
            string filePath,
            bool monitorFileChanges
        ) : base(filePath, monitorFileChanges) {
        }

        public override void Load(bool changeDetected) {
            if (File.Exists(FilePath)) {
                try {
                    string dataJson = File.ReadAllText(FilePath);
                    InstanceData = JsonUtility.FromJson<TData>(dataJson);
                } catch (Exception e) {
                    Debug.LogError($"Error loading JSON from {FilePath}:\n{e}");
                }

                return;
            }

            InstanceData = new TData();
            Save();
        }

        protected override void SaveInstanceData() {
            string dataJson = JsonUtility.ToJson(InstanceData, true);
            File.WriteAllText(FilePath, dataJson);
        }

        public override void Delete() {
            base.Delete();
            InstanceData = new TData();
        }
    }
}
