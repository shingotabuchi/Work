using DG.Tweening;
using UnityEngine;

namespace Fwk.UI
{
    public class UIAnimationLoop : MonoBehaviour
    {
        [SerializeField] private AnimationCurve xCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve yCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private float duration = 1f;
        [SerializeField] private float xMultiplier = 100f;
        [SerializeField] private float yMultiplier = 100f;
        [SerializeField] private bool startOnEnable = true;

        private Tween xTween;
        private Tween yTween;
        private Vector3 originalPosition;

        void Awake()
        {
            originalPosition = transform.localPosition;
        }

        void OnEnable()
        {
            if (startOnEnable)
            {
                StartAnimation();
            }
        }

        void OnDisable()
        {
            StopAnimation();
        }

        public void StartAnimation()
        {
            StopAnimation();

            // Animate X position based on curve
            xTween = DOTween.To(
                () => 0f,
                (value) =>
                {
                    Vector3 pos = transform.localPosition;
                    pos.x = originalPosition.x + xCurve.Evaluate(value) * xMultiplier;
                    transform.localPosition = pos;
                },
                1f,
                duration
            ).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);

            // Animate Y position based on curve
            yTween = DOTween.To(
                () => 0f,
                (value) =>
                {
                    Vector3 pos = transform.localPosition;
                    pos.y = originalPosition.y + yCurve.Evaluate(value) * yMultiplier;
                    transform.localPosition = pos;
                },
                1f,
                duration
            ).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        }

        public void StopAnimation()
        {
            if (xTween != null && xTween.IsActive())
                xTween.Kill();

            if (yTween != null && yTween.IsActive())
                yTween.Kill();

            // Reset to original position
            transform.localPosition = originalPosition;
        }

        public void PauseAnimation()
        {
            xTween?.Pause();
            yTween?.Pause();
        }

        public void ResumeAnimation()
        {
            xTween?.Play();
            yTween?.Play();
        }
    }
}