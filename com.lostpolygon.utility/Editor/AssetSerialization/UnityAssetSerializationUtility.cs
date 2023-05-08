using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace LostPolygon.Unity.Utility.Editor {
    public static class UnityAssetSerializationUtility {
        public static Dictionary<string, object> GetAssetPersistentReferenceJson(string guid, long localIdentifier) {
            return new Dictionary<string, object> {
                { "Guid", guid },
                { "LocalIdentifier", localIdentifier }
            };
        }

        public static AssetReference? GetAssetPersistentReference(Object unityObject) {
            if (unityObject == null)
                return null;

            bool success = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(unityObject, out string guid, out long localIdentifier);
            if (!success)
                return null;

            return new AssetReference(guid, localIdentifier);
        }

        public static TLazyReferenceType CreateAssetLazyReference<TLazyReferenceType>(
            Object unityObject,
            Func<AssetReference, int, TLazyReferenceType> createFunc
        ) where TLazyReferenceType : class {
            AssetReference? assetReference = GetAssetPersistentReference(unityObject);
            if (assetReference == null)
                return null;

            TLazyReferenceType lazyReference = createFunc(assetReference.Value, unityObject.GetInstanceID());
            return lazyReference;
        }
    }
}
