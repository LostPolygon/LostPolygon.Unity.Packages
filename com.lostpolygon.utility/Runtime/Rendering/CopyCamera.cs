using UnityEngine;

namespace LostPolygon.Unity.Utility {
    [RequireComponent(typeof(Camera))]
    public class CopyCamera : MonoBehaviour {
        [SerializeField]
        private Camera _sourceCamera;

        [SerializeField]
        private bool _copyDepth;

        [SerializeField]
        private bool _copyCullingMask;

        [SerializeField]
        private bool _copyClearFlags;

        private Camera _camera;

        private void OnEnable() {
            _camera = GetComponent<Camera>();
            Copy();
        }

        private void Update() {
            Copy();
        }

        private void ManualRefresh() {
            _camera = GetComponent<Camera>();
            Copy();
        }

        private void Copy() {
            CameraClearFlags cameraClearFlags = _camera.clearFlags;
            int cameraCullingMask = _camera.cullingMask;
            float cameraDepth = _camera.depth;

            _camera.CopyFrom(_sourceCamera);

            if (!_copyClearFlags) {
                _camera.clearFlags = cameraClearFlags;
            }

            if (!_copyCullingMask) {
                _camera.cullingMask = cameraCullingMask;
            }

            if (!_copyDepth) {
                _camera.depth = cameraDepth;
            }
        }
    }
}
