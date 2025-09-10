namespace echo17.EnhancedUI.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The easing type
    /// </summary>
    public enum TweenType
    {
        immediate,
        linear,
        spring,
        easeInQuad,
        easeOutQuad,
        easeInOutQuad,
        easeInCubic,
        easeOutCubic,
        easeInOutCubic,
        easeInQuart,
        easeOutQuart,
        easeInOutQuart,
        easeInQuint,
        easeOutQuint,
        easeInOutQuint,
        easeInSine,
        easeOutSine,
        easeInOutSine,
        easeInExpo,
        easeOutExpo,
        easeInOutExpo,
        easeInCirc,
        easeOutCirc,
        easeInOutCirc,
        easeInBounce,
        easeOutBounce,
        easeInOutBounce,
        easeInBack,
        easeOutBack,
        easeInOutBack,
        easeInElastic,
        easeOutElastic,
        easeInOutElastic
    }

    /// <summary>
    /// Helper class to modify a value over time based on an easing function and interval
    /// </summary>
    public static class EnhancedTween
    {
        /// <summary>
        /// Change a value based on a starting value, ending value, point between that spectrum,
        /// and the calculation used to modify the value.
        /// </summary>
        /// <param name="tweenType">The calculations used to determine the values change</param>
        /// <param name="start">The beginning value</param>
        /// <param name="end">The final value</param>
        /// <param name="percentage">Normalized parameter between start and end. Should be between 0 and 1</param>
        /// <returns>Modified value</returns>
        public static float TweenFloat(TweenType tweenType, float start, float end, float percentage)
        {
            percentage = Mathf.Clamp01(percentage);
            if (percentage == 0f) return start;
            if (percentage == 1f) return end;

            float newValue = start;

            switch (tweenType)
            {
                case TweenType.linear: newValue = linear(start, end, percentage); break;
                case TweenType.spring: newValue = spring(start, end, percentage); break;
                case TweenType.easeInQuad: newValue = easeInQuad(start, end, percentage); break;
                case TweenType.easeOutQuad: newValue = easeOutQuad(start, end, percentage); break;
                case TweenType.easeInOutQuad: newValue = easeInOutQuad(start, end, percentage); break;
                case TweenType.easeInCubic: newValue = easeInCubic(start, end, percentage); break;
                case TweenType.easeOutCubic: newValue = easeOutCubic(start, end, percentage); break;
                case TweenType.easeInOutCubic: newValue = easeInOutCubic(start, end, percentage); break;
                case TweenType.easeInQuart: newValue = easeInQuart(start, end, percentage); break;
                case TweenType.easeOutQuart: newValue = easeOutQuart(start, end, percentage); break;
                case TweenType.easeInOutQuart: newValue = easeInOutQuart(start, end, percentage); break;
                case TweenType.easeInQuint: newValue = easeInQuint(start, end, percentage); break;
                case TweenType.easeOutQuint: newValue = easeOutQuint(start, end, percentage); break;
                case TweenType.easeInOutQuint: newValue = easeInOutQuint(start, end, percentage); break;
                case TweenType.easeInSine: newValue = easeInSine(start, end, percentage); break;
                case TweenType.easeOutSine: newValue = easeOutSine(start, end, percentage); break;
                case TweenType.easeInOutSine: newValue = easeInOutSine(start, end, percentage); break;
                case TweenType.easeInExpo: newValue = easeInExpo(start, end, percentage); break;
                case TweenType.easeOutExpo: newValue = easeOutExpo(start, end, percentage); break;
                case TweenType.easeInOutExpo: newValue = easeInOutExpo(start, end, percentage); break;
                case TweenType.easeInCirc: newValue = easeInCirc(start, end, percentage); break;
                case TweenType.easeOutCirc: newValue = easeOutCirc(start, end, percentage); break;
                case TweenType.easeInOutCirc: newValue = easeInOutCirc(start, end, percentage); break;
                case TweenType.easeInBounce: newValue = easeInBounce(start, end, percentage); break;
                case TweenType.easeOutBounce: newValue = easeOutBounce(start, end, percentage); break;
                case TweenType.easeInOutBounce: newValue = easeInOutBounce(start, end, percentage); break;
                case TweenType.easeInBack: newValue = easeInBack(start, end, percentage); break;
                case TweenType.easeOutBack: newValue = easeOutBack(start, end, percentage); break;
                case TweenType.easeInOutBack: newValue = easeInOutBack(start, end, percentage); break;
                case TweenType.easeInElastic: newValue = easeInElastic(start, end, percentage); break;
                case TweenType.easeOutElastic: newValue = easeOutElastic(start, end, percentage); break;
                case TweenType.easeInOutElastic: newValue = easeInOutElastic(start, end, percentage); break;
            }

            return newValue;
        }

        private static float linear(float start, float end, float percentage)
        {
            return Mathf.Lerp(start, end, percentage);
        }

        private static float spring(float start, float end, float percentage)
        {
            percentage = Mathf.Clamp01(percentage);
            percentage = (Mathf.Sin(percentage * Mathf.PI * (0.2f + 2.5f * percentage * percentage * percentage)) * Mathf.Pow(1f - percentage, 2.2f) + percentage) * (1f + (1.2f * (1f - percentage)));
            return start + (end - start) * percentage;
        }

        private static float easeInQuad(float start, float end, float percentage)
        {
            end -= start;
            return end * percentage * percentage + start;
        }

        private static float easeOutQuad(float start, float end, float percentage)
        {
            end -= start;
            return -end * percentage * (percentage - 2) + start;
        }

        private static float easeInOutQuad(float start, float end, float percentage)
        {
            percentage /= .5f;
            end -= start;
            if (percentage < 1) return end / 2 * percentage * percentage + start;
            percentage--;
            return -end / 2 * (percentage * (percentage - 2) - 1) + start;
        }

        private static float easeInCubic(float start, float end, float percentage)
        {
            end -= start;
            return end * percentage * percentage * percentage + start;
        }

        private static float easeOutCubic(float start, float end, float percentage)
        {
            percentage--;
            end -= start;
            return end * (percentage * percentage * percentage + 1) + start;
        }

        private static float easeInOutCubic(float start, float end, float percentage)
        {
            percentage /= .5f;
            end -= start;
            if (percentage < 1) return end / 2 * percentage * percentage * percentage + start;
            percentage -= 2;
            return end / 2 * (percentage * percentage * percentage + 2) + start;
        }

        private static float easeInQuart(float start, float end, float percentage)
        {
            end -= start;
            return end * percentage * percentage * percentage * percentage + start;
        }

        private static float easeOutQuart(float start, float end, float percentage)
        {
            percentage--;
            end -= start;
            return -end * (percentage * percentage * percentage * percentage - 1) + start;
        }

        private static float easeInOutQuart(float start, float end, float percentage)
        {
            percentage /= .5f;
            end -= start;
            if (percentage < 1) return end / 2 * percentage * percentage * percentage * percentage + start;
            percentage -= 2;
            return -end / 2 * (percentage * percentage * percentage * percentage - 2) + start;
        }

        private static float easeInQuint(float start, float end, float percentage)
        {
            end -= start;
            return end * percentage * percentage * percentage * percentage * percentage + start;
        }

        private static float easeOutQuint(float start, float end, float percentage)
        {
            percentage--;
            end -= start;
            return end * (percentage * percentage * percentage * percentage * percentage + 1) + start;
        }

        private static float easeInOutQuint(float start, float end, float percentage)
        {
            percentage /= .5f;
            end -= start;
            if (percentage < 1) return end / 2 * percentage * percentage * percentage * percentage * percentage + start;
            percentage -= 2;
            return end / 2 * (percentage * percentage * percentage * percentage * percentage + 2) + start;
        }

        private static float easeInSine(float start, float end, float percentage)
        {
            end -= start;
            return -end * Mathf.Cos(percentage / 1 * (Mathf.PI / 2)) + end + start;
        }

        private static float easeOutSine(float start, float end, float percentage)
        {
            end -= start;
            return end * Mathf.Sin(percentage / 1 * (Mathf.PI / 2)) + start;
        }

        private static float easeInOutSine(float start, float end, float percentage)
        {
            end -= start;
            return -end / 2 * (Mathf.Cos(Mathf.PI * percentage / 1) - 1) + start;
        }

        private static float easeInExpo(float start, float end, float percentage)
        {
            end -= start;
            return end * Mathf.Pow(2, 10 * (percentage / 1 - 1)) + start;
        }

        private static float easeOutExpo(float start, float end, float percentage)
        {
            end -= start;
            return end * (-Mathf.Pow(2, -10 * percentage / 1) + 1) + start;
        }

        private static float easeInOutExpo(float start, float end, float percentage)
        {
            percentage /= .5f;
            end -= start;
            if (percentage < 1) return end / 2 * Mathf.Pow(2, 10 * (percentage - 1)) + start;
            percentage--;
            return end / 2 * (-Mathf.Pow(2, -10 * percentage) + 2) + start;
        }

        private static float easeInCirc(float start, float end, float percentage)
        {
            end -= start;
            return -end * (Mathf.Sqrt(1 - percentage * percentage) - 1) + start;
        }

        private static float easeOutCirc(float start, float end, float percentage)
        {
            percentage--;
            end -= start;
            return end * Mathf.Sqrt(1 - percentage * percentage) + start;
        }

        private static float easeInOutCirc(float start, float end, float percentage)
        {
            percentage /= .5f;
            end -= start;
            if (percentage < 1) return -end / 2 * (Mathf.Sqrt(1 - percentage * percentage) - 1) + start;
            percentage -= 2;
            return end / 2 * (Mathf.Sqrt(1 - percentage * percentage) + 1) + start;
        }

        private static float easeInBounce(float start, float end, float percentage)
        {
            end -= start;
            float d = 1f;
            return end - easeOutBounce(0, end, d - percentage) + start;
        }

        private static float easeOutBounce(float start, float end, float percentage)
        {
            percentage /= 1f;
            end -= start;
            if (percentage < (1 / 2.75f))
            {
                return end * (7.5625f * percentage * percentage) + start;
            }
            else if (percentage < (2 / 2.75f))
            {
                percentage -= (1.5f / 2.75f);
                return end * (7.5625f * (percentage) * percentage + .75f) + start;
            }
            else if (percentage < (2.5 / 2.75))
            {
                percentage -= (2.25f / 2.75f);
                return end * (7.5625f * (percentage) * percentage + .9375f) + start;
            }
            else
            {
                percentage -= (2.625f / 2.75f);
                return end * (7.5625f * (percentage) * percentage + .984375f) + start;
            }
        }

        private static float easeInOutBounce(float start, float end, float percentage)
        {
            end -= start;
            float d = 1f;
            if (percentage < d / 2) return easeInBounce(0, end, percentage * 2) * 0.5f + start;
            else return easeOutBounce(0, end, percentage * 2 - d) * 0.5f + end * 0.5f + start;
        }

        private static float easeInBack(float start, float end, float percentage)
        {
            end -= start;
            percentage /= 1;
            float s = 1.70158f;
            return end * (percentage) * percentage * ((s + 1) * percentage - s) + start;
        }

        private static float easeOutBack(float start, float end, float percentage)
        {
            float s = 1.70158f;
            end -= start;
            percentage = (percentage / 1) - 1;
            return end * ((percentage) * percentage * ((s + 1) * percentage + s) + 1) + start;
        }

        private static float easeInOutBack(float start, float end, float percentage)
        {
            float s = 1.70158f;
            end -= start;
            percentage /= .5f;
            if ((percentage) < 1)
            {
                s *= (1.525f);
                return end / 2 * (percentage * percentage * (((s) + 1) * percentage - s)) + start;
            }
            percentage -= 2;
            s *= (1.525f);
            return end / 2 * ((percentage) * percentage * (((s) + 1) * percentage + s) + 2) + start;
        }

        private static float easeInElastic(float start, float end, float percentage)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (percentage == 0) return start;
            percentage = percentage / d;
            if (percentage == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }
            percentage = percentage - 1;
            return -(a * Mathf.Pow(2, 10 * percentage) * Mathf.Sin((percentage * d - s) * (2 * Mathf.PI) / p)) + start;
        }

        private static float easeOutElastic(float start, float end, float percentage)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (percentage == 0) return start;

            percentage = percentage / d;
            if (percentage == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return (a * Mathf.Pow(2, -10 * percentage) * Mathf.Sin((percentage * d - s) * (2 * Mathf.PI) / p) + end + start);
        }

        private static float easeInOutElastic(float start, float end, float percentage)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (percentage == 0) return start;

            percentage = percentage / (d / 2);
            if (percentage == 2) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            if (percentage < 1)
            {
                percentage = percentage - 1;
                return -0.5f * (a * Mathf.Pow(2, 10 * percentage) * Mathf.Sin((percentage * d - s) * (2 * Mathf.PI) / p)) + start;
            }
            percentage = percentage - 1;
            return a * Mathf.Pow(2, -10 * percentage) * Mathf.Sin((percentage * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
        }
    }
}
