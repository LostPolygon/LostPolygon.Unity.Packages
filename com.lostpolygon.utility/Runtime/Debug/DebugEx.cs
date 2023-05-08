using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace LostPolygon.Unity.Utility {
    public static class DebugEx {
        /// <summary>
        ///   Draw a wire sphere
        /// </summary>
        /// <param name="center"> </param>
        /// <param name="radius"> </param>
        /// <param name="color"> </param>
        /// <param name="duration"> </param>
        /// <param name="depthTest"> </param>
        /// <param name="quality"> Define the quality of the wire sphere, from 1 to 10 </param>
        /// https://www.reddit.com/r/Unity3D/comments/mkxe7m/a_way_to_visualize_wire_spheres_in_debug/
        [Conditional("UNITY_EDITOR")]
        public static void DrawWireSphere(Vector3 center, float radius, Color color, float duration, bool depthTest = true, int quality = 2) {
            quality = Mathf.Clamp(quality, 1, 10);

            int segments = quality << 2;
            int subdivisions = quality << 3;
            int halfSegments = segments >> 1;
            float strideAngle = 360f / subdivisions;
            float segmentStride = 180f / segments;

            Vector3 first;
            Vector3 next;
            for (int i = 0; i < segments; i++) {
                first = Vector3.forward * radius;
                first = Quaternion.AngleAxis(segmentStride * (i - halfSegments), Vector3.right) * first;

                for (int j = 0; j < subdivisions; j++) {
                    next = Quaternion.AngleAxis(strideAngle, Vector3.up) * first;
                    Debug.DrawLine(first + center, next + center, color, duration, depthTest);
                    first = next;
                }
            }

            for (int i = 0; i < segments; i++) {
                first = Vector3.forward * radius;
                first = Quaternion.AngleAxis(segmentStride * (i - halfSegments), Vector3.up) * first;
                Vector3 axis = Quaternion.AngleAxis(90f, Vector3.up) * first;

                for (int j = 0; j < subdivisions; j++) {
                    next = Quaternion.AngleAxis(strideAngle, axis) * first;
                    Debug.DrawLine(first + center, next + center, color, duration, depthTest);
                    first = next;
                }
            }
        }
    }
}
