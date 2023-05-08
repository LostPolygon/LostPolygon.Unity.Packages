using UnityEngine;
using Random = UnityEngine.Random;

namespace LostPolygon.Unity.Utility {
    public class BoundsGizmoDrawer : MonoBehaviour {
        [SerializeField]
        private bool _onlyWhenSelected;

        [SerializeField]
        private Color _color = Color.red;

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (_onlyWhenSelected)
                return;

            DrawGizmos();
        }

        private void OnDrawGizmosSelected() {
            DrawGizmos();
        }

        private void DrawGizmos() {
            Gizmos.color = _color;

            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null) {
                Vector3[] fourCorners = new Vector3[4];
                rectTransform.GetWorldCorners(fourCorners);

                Bounds bounds = new Bounds {
                    center = fourCorners[0]
                };
                bounds.Encapsulate(fourCorners[1]);
                bounds.Encapsulate(fourCorners[2]);
                bounds.Encapsulate(fourCorners[3]);
                Gizmos.DrawCube(bounds.center, bounds.size);
                return;
            }

            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null) {
                Gizmos.DrawCube(transform.position, meshRenderer.bounds.size);
                return;
            }

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
        }

        private void Reset() {
            _color = Random.ColorHSV(0, 1).WithAlpha(0.25f);
        }
#endif
    }
}
