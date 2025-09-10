using DG.Tweening;
using UnityEngine;

namespace Fwk.UI
{
    public class UIShake : MonoBehaviour
    {
        [SerializeField] private float shakeDuration = 1f;
        [SerializeField] private float shakeStrength = 0.5f;
        [SerializeField] private int shakeVibrato = 10;
        [SerializeField] private float shakeRandomness = 90f;
        [SerializeField] private bool shakeSnapping = false;
        [SerializeField] private bool shakeFadeOut = false;

        private Tween shakeTween;

        void OnEnable()
        {
            // Start shake when enabled
            shakeTween = transform.DOShakePosition(
                duration: shakeDuration,
                strength: new Vector3(shakeStrength, shakeStrength, 0f),
                vibrato: shakeVibrato,
                randomness: shakeRandomness,
                snapping: shakeSnapping,
                fadeOut: shakeFadeOut
            ).SetLoops(-1, LoopType.Restart);
        }

        void OnDisable()
        {
            // Kill tween when disabled to stop shaking
            if (shakeTween != null && shakeTween.IsActive())
                shakeTween.Kill();
        }
    }
}