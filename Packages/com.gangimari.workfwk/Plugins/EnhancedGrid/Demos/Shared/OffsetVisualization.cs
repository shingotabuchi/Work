namespace echo17.EnhancedUI.EnhancedGrid.Demos
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class OffsetVisualization : MonoBehaviour
    {
        public RectTransform viewportRectTransform;
        public RectTransform x;
        public RectTransform y;
        public RectTransform reticle;
        public Slider xSlider;
        public Slider ySlider;

        private Vector2 _reticleHalfSize;
        private float _xHalfSize;
        private float _yHalfSize;
        private const float _TimeBetweenScreenChangeCalculations = 0.5f;
        private float _lastScreenChangeCalculationTime = 0;
        private bool _resetScheduled = false;

        private void Awake()
        {
            _reticleHalfSize = reticle.rect.size / 2.0f;
            _xHalfSize = x.rect.width / 2.0f;
            _yHalfSize = y.rect.height / 2.0f;
            _lastScreenChangeCalculationTime = Time.time;

            _SetOffsetVisualization();
        }

        private void LateUpdate()
        {
            if (_resetScheduled && viewportRectTransform != null)
            {
                _SetOffsetVisualization();
                _resetScheduled = false;
            }
        }

        private void OnRectTransformDimensionsChange()
        {
            if (Time.time - _lastScreenChangeCalculationTime < _TimeBetweenScreenChangeCalculations)
                return;

            _lastScreenChangeCalculationTime = Time.time;

            _resetScheduled = true;
        }

        public void XSlider_OnValueChanged(Slider slider)
        {
            _SetOffsetVisualization();
        }

        public void YSlider_OnValueChanged(Slider slider)
        {
            _SetOffsetVisualization();
        }

        private void _SetOffsetVisualization()
        {
            if (viewportRectTransform == null) return;

            x.localPosition = new Vector3((xSlider.value * viewportRectTransform.rect.size.x) - _xHalfSize, 0, 0);
            y.localPosition = new Vector3(0, -(ySlider.value * viewportRectTransform.rect.size.y) + _yHalfSize, 0);

            reticle.localPosition = new Vector3(
                                                    (xSlider.value * viewportRectTransform.rect.size.x) - _reticleHalfSize.x,
                                                    -(ySlider.value * viewportRectTransform.rect.size.y) + _reticleHalfSize.y,
                                                    0
                                                );
        }
    }
}