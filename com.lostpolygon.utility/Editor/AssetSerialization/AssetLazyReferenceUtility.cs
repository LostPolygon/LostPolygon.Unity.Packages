using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace LostPolygon.Unity.Utility.Editor {
    public static class AssetLazyReferenceUtility {
        public static T GetInstance<T>(
            Func<T> loadFunc,
            ref int cachedInstanceId
        ) where T : Object {
            T instance;
            if (cachedInstanceId == 0) {
                instance = loadFunc();
                cachedInstanceId = instance == null ? 0 : instance.GetInstanceID();
                return instance;
            }

            instance = EditorUtility.InstanceIDToObject(cachedInstanceId) as T;
            if (instance != null)
                return instance;

            instance = loadFunc();
            cachedInstanceId = instance == null ? 0 : instance.GetInstanceID();

            return instance;
        }
    }
}
