using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;

namespace Fwk
{
    public class CameraManager : SingletonPersistent<CameraManager>
    {
        public Camera MainCamera { get; private set; }
        public Camera UICamera { get; private set; }
        public Camera FXCamera { get; private set; }

        private int _fxCount = 0;

        protected override void Awake()
        {
            base.Awake();
            UpdateCameras();
        }

        public static void CreateIfNotExists()
        {
            if (Instance != null)
            {
                return;
            }
            var go = new GameObject("CameraManager");
            Instance = go.AddComponent<CameraManager>();
            go.AddComponent<AudioListener>();
        }

        public void UpdateCameras()
        {
            FindAndSetMainCamera();
            FindAndSetUICamera();
            FindAndSetFXCamera();
            SetCanvasCameras();
            DestroyOtherCameras();
            SetCameraStack();
            SetFXCameraActive(false);
        }

        private void SetCameraStack()
        {
            if (MainCamera != null && UICamera != null)
            {
                var mainCameraStack = MainCamera.GetUniversalAdditionalCameraData();
                if (!mainCameraStack.cameraStack.Contains(UICamera))
                {
                    mainCameraStack.cameraStack.Add(UICamera);
                }

                if (!mainCameraStack.cameraStack.Contains(FXCamera))
                {
                    mainCameraStack.cameraStack.Add(FXCamera);
                }
            }
        }

        private void SetCanvasCameras()
        {
            var canvasCameras = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (var canvas in canvasCameras)
            {
                if (canvas.worldCamera == null)
                {
                    continue;
                }
                if (canvas.worldCamera.gameObject.CompareTag("MainCamera"))
                {
                    canvas.worldCamera = MainCamera;
                }
                else if (canvas.worldCamera.gameObject.CompareTag("FXCamera"))
                {
                    canvas.worldCamera = FXCamera;
                }
                else
                {
                    canvas.worldCamera = UICamera;
                }
            }
        }
        private void FindAndSetMainCamera()
        {
            if (MainCamera != null)
            {
                return;
            }

            var mainCameraObject = GameObject.FindWithTag("MainCamera");
            if (mainCameraObject != null)
            {
                if (mainCameraObject.TryGetComponent<AudioListener>(out var listener))
                {
                    Destroy(listener);
                }

                MainCamera = mainCameraObject.GetComponent<Camera>();
                MainCamera.transform.SetParent(transform, false);
            }
        }

        private void FindAndSetUICamera()
        {
            if (UICamera != null)
            {
                return;
            }
            var uiCameraObject = GameObject.FindWithTag("UICamera");
            if (uiCameraObject != null)
            {
                if (uiCameraObject.TryGetComponent<AudioListener>(out var listener))
                {
                    Destroy(listener);
                }

                UICamera = uiCameraObject.GetComponent<Camera>();
                UICamera.transform.SetParent(transform, false);
            }
        }

        private void FindAndSetFXCamera()
        {
            if (FXCamera != null)
            {
                return;
            }
            var fxCameraObject = GameObject.FindWithTag("FXCamera");
            if (fxCameraObject != null)
            {
                if (fxCameraObject.TryGetComponent<AudioListener>(out var listener))
                {
                    Destroy(listener);
                }

                FXCamera = fxCameraObject.GetComponent<Camera>();
                FXCamera.transform.SetParent(transform, false);
            }
        }

        private void DestroyOtherCameras()
        {
            var foundCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
            foreach (var camera in foundCameras)
            {
                if (camera != MainCamera && camera != UICamera && camera != FXCamera)
                {
                    Destroy(camera.gameObject);
                }
            }
        }

        public void SetFXCameraActive(bool active)
        {
            if (FXCamera != null)
            {
                FXCamera.gameObject.SetActive(active);
            }
        }

        public UniTask SetFXCameraActiveAsync(bool active, float delay)
        {
            return UniTask.Delay(TimeSpan.FromSeconds(delay)).ContinueWith(() => SetFXCameraActive(active));
        }
    }
}