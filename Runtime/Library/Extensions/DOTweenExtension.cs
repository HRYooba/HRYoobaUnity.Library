using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HRYooba.Library
{
    public static class DOTweenExtension
    {
        public static Tween DOFloat(this float startValue, float endValue, float duration)
        {
            return DOTween.To(() => startValue, value => startValue = value, endValue, duration);
        }

        public static Tween DOVector3(this Vector3 startValue, Vector3 endValue, float duration)
        {
            return DOTween.To(() => startValue, value => startValue = value, endValue, duration);
        }

        public static Tween DOVector2(this Vector2 startValue, Vector2 endValue, float duration)
        {
            return DOTween.To(() => startValue, value => startValue = value, endValue, duration);
        }

        public static Tween DOColor(this Color startValue, Color endValue, float duration)
        {
            return DOTween.To(() => startValue, value => startValue = value, endValue, duration);
        }
    }
}