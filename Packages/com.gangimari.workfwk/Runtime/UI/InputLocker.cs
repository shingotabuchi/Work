using UnityEngine;
using UnityEngine.UI;

namespace Fwk.UI
{
    public class InputLocker : SingletonPersistent<InputLocker>
    {
        private int _lockCount = 0;
        private CanvasGroup _overlayGroup;

        public static void CreateIfNotExists()
        {
            if (Instance != null)
            {
                return;
            }
            var go = new GameObject("InputLocker");
            Instance = go.AddComponent<InputLocker>();
        }

        protected override void Awake()
        {
            base.Awake();

            var overlay = new GameObject("InputLockCanvas");
            overlay.transform.SetParent(transform, false);
            var canvas = overlay.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = int.MaxValue;

            overlay.AddComponent<CanvasScaler>();
            overlay.AddComponent<GraphicRaycaster>();
            _overlayGroup = overlay.AddComponent<CanvasGroup>();
            _overlayGroup.interactable = false;
            _overlayGroup.blocksRaycasts = false;

            var imageGO = new GameObject("InputBlockImage");
            imageGO.transform.SetParent(overlay.transform, false);
            var img = imageGO.AddComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0f);

            var rt = img.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        public void Lock()
        {
            _lockCount++;
            _overlayGroup.blocksRaycasts = true;
        }

        public void Unlock()
        {
            if (_lockCount <= 0)
                return;

            _lockCount--;
            if (_lockCount == 0)
            {
                _overlayGroup.blocksRaycasts = false;
            }
        }
    }
}