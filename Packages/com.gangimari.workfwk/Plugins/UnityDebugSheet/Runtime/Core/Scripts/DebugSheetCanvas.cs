using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts
{
    [ExecuteAlways]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [DefaultExecutionOrder(-10)]
    public sealed class DebugSheetCanvas : MonoBehaviour
    {
        [SerializeField] private CanvasScaler _canvasScaler;
        [SerializeField][Range(0.1f, 1.0f)] private float _widthScale = 1.0f;

        [Header("FPS Display")]
        [SerializeField] private GameObject _fpsPanel;
        [SerializeField] private Text _currentFpsText;
        [SerializeField] private Text _minFpsText;
        [SerializeField] private Text _avgFpsText;

        private bool _isPortrait;
        private bool _showFps = false;

        // FPS tracking variables
        private float _deltaTime = 0.0f;
        private float _updateTimer = 0.0f;
        private const float UpdateInterval = 1.0f;

        private float _currentFps = 0.0f;
        private float _minFps = float.MaxValue;
        private float _totalFps = 0.0f;
        private int _frameCount = 0;
        private float _avgFps = 0.0f;

        private void Awake()
        {
#if UNITY_EDITOR
            _canvasScaler = GetComponent<CanvasScaler>();
#endif
            Apply(true);
            InitializeFpsDisplay();
        }

        private void Update()
        {
            Apply();

            if (_showFps)
            {
                UpdateFpsTracking();
            }
        }

        private void InitializeFpsDisplay()
        {
            if (_fpsPanel != null)
            {
                _fpsPanel.SetActive(_showFps);
            }
        }

        private void UpdateFpsTracking()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
            _currentFps = 1.0f / _deltaTime;

            // Track min and max
            if (_currentFps < _minFps && _currentFps > 0)
                _minFps = _currentFps;

            // Calculate average
            _totalFps += _currentFps;
            _frameCount++;

            _updateTimer += Time.unscaledDeltaTime;

            if (_updateTimer >= UpdateInterval)
            {
                _avgFps = _totalFps / _frameCount;
                UpdateFpsDisplay();
                _updateTimer = 0.0f;
            }
        }

        private void UpdateFpsDisplay()
        {
            if (_currentFpsText != null)
                _currentFpsText.text = $"FPS: {_currentFps:F1}";

            if (_minFpsText != null)
                _minFpsText.text = $"Min: {_minFps:F1}";

            if (_avgFpsText != null)
                _avgFpsText.text = $"Avg: {_avgFps:F1}";
        }

        public void SetFpsDisplayEnabled(bool enabled)
        {
            _showFps = enabled;

            if (_fpsPanel != null)
            {
                _fpsPanel.SetActive(_showFps);
            }

            if (_showFps)
            {
                ResetFpsStats();
            }
        }

        private void ResetFpsStats()
        {
            _minFps = float.MaxValue;
            _totalFps = 0.0f;
            _frameCount = 0;
            _avgFps = 0.0f;
            _updateTimer = 0.0f;
        }

        private void Apply(bool force = false)
        {
            var isPortrait = Screen.height >= Screen.width;
#if !UNITY_EDITOR
            if (!force && _isPortrait == isPortrait)
            {
                return;
            }
#endif

            var referenceResolution = isPortrait ? new Vector2(750, 1334) : new Vector2(1334, 750);
            if (isPortrait)
                referenceResolution.x *= 1 / _widthScale;
            else
                referenceResolution.y *= 1 / _widthScale;
            _canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _canvasScaler.referenceResolution = referenceResolution;
            _canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            _canvasScaler.matchWidthOrHeight = isPortrait ? 0.0f : 1.0f;

            _isPortrait = isPortrait;
        }
    }
}
