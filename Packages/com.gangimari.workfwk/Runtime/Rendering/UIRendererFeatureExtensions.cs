using UnityEngine;
using DG.Tweening;

public static class UIRendererFeatureExtensions
{
    // Static ID to identify blur rate tweens
    private static readonly string BlurRateTweenId = "UIBlurRateTween";

    /// <summary>
    /// DOTween animation for turning UI blur on.
    /// </summary>
    /// <param name="endValue">Target blur rate (0 to 1)</param>
    /// <param name="duration">Animation duration in seconds</param>
    /// <returns>The Tweener that animates the blur rate</returns>
    public static Tweener DOBlurRateOn(this UIRendererFeature feature, float duration, float endValue = 1f)
    {
        if (!UIRendererFeature.ExistsInstance())
        {
            Debug.LogWarning("UIRendererFeature instance does not exist.");
            return null;
        }

        // Kill any existing blur rate tweens to prevent conflicts
        DOTween.Kill(BlurRateTweenId);

        endValue = Mathf.Clamp01(endValue);

        // Set ExecBlur to true immediately
        feature.SetExecBlur(true);

        var tween = DOTween.To(
            () => feature.GetBlurRate(),
            x => feature.SetBlurRate(x),
            endValue,
            duration)
            .SetId(BlurRateTweenId);

        tween.OnComplete(() =>
        {
            feature.SetBlurRate(endValue);
            feature.SetExecBlur(true);
        });

        return tween;
    }

    /// <summary>
    /// DOTween animation for turning UI blur off.
    /// </summary>
    /// <param name="endValue">Target blur rate to reach before turning off (usually 0)</param>
    /// <param name="duration">Animation duration in seconds</param>
    /// <returns>The Tweener that animates the blur rate</returns>
    public static Tweener DOBlurRateOff(this UIRendererFeature feature, float duration, float endValue = 0f)
    {
        if (!UIRendererFeature.ExistsInstance())
        {
            Debug.LogWarning("UIRendererFeature instance does not exist.");
            return null;
        }

        // Kill any existing blur rate tweens to prevent conflicts
        DOTween.Kill(BlurRateTweenId);

        endValue = Mathf.Clamp01(endValue);

        var tween = DOTween.To(
            () => feature.GetBlurRate(),
            x => feature.SetBlurRate(x),
            endValue,
            duration)
            .SetId(BlurRateTweenId);

        tween.OnComplete(() =>
        {
            feature.SetBlurRate(endValue);
            feature.SetExecBlur(false);
        });

        return tween;
    }
}