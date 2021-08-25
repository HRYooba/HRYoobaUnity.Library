using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HRYooba.Library
{
    public static class DOTweenExtension
    {
        public static Tween DOFloat(this FloatTweenProperty target, float endValue, float duration)
        {
            return DOTween.To(() => target.Value, value => target.Value = value, endValue, duration);
        }

        public static Tween DOVector3(this Vector3TweenProperty target, Vector3 endValue, float duration)
        {
            return DOTween.To(() => target.Value, value => target.Value = value, endValue, duration);
        }

        public static Tween DOVector2(this Vector2TweenProperty target, Vector2 endValue, float duration)
        {
            return DOTween.To(() => target.Value, value => target.Value = value, endValue, duration);
        }

        public static Tween DOColor(this ColorTweenProperty target, Color endValue, float duration)
        {
            return DOTween.To(() => target.Value, value => target.Value = value, endValue, duration);
        }
    }

    [System.Serializable]
    public class FloatTweenProperty
    {
        [SerializeField]
        private float _value;

        public FloatTweenProperty(float value = default)
        {
            _value = value;
        }

        public float Value
        {
            set { _value = value; }
            get { return _value; }
        }
    }

    public class Vector2TweenProperty
    {
        [SerializeField]
        private Vector2 _value;

        public Vector2TweenProperty(Vector2 value = default)
        {
            _value = value;
        }

        public Vector2 Value
        {
            set { _value = value; }
            get { return _value; }
        }
    }

    public class Vector3TweenProperty
    {
        [SerializeField]
        private Vector3 _value;

        public Vector3TweenProperty(Vector3 value = default)
        {
            _value = value;
        }

        public Vector3 Value
        {
            set { _value = value; }
            get { return _value; }
        }
    }

    public class ColorTweenProperty
    {
        [SerializeField]
        private Color _value;

        public ColorTweenProperty(Color value = default)
        {
            _value = value;
        }

        public Color Value
        {
            set { _value = value; }
            get { return _value; }
        }
    }
}