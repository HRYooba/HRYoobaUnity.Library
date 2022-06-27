using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace HRYooba.UI
{
    [ExecuteInEditMode]
    public class FloatController : MonoBehaviour
    {
        [Header("Value")]
        [SerializeField] private StringReactiveProperty _title = new StringReactiveProperty("FloatController");
        [SerializeField] private FloatReactiveProperty _value = new FloatReactiveProperty(0.0f);
        [SerializeField] private FloatReactiveProperty _maxValue = new FloatReactiveProperty(1.0f);
        [SerializeField] private FloatReactiveProperty _minValue = new FloatReactiveProperty(0.0f);

        [Header("UI")]
        [SerializeField] private Slider _slider = null;
        [SerializeField] private InputField _inputField = null;
        [SerializeField] private Text _text = null;

        public System.IObservable<float> OnValueChanged
        {
            get { return _value; }
        }

        public string Title
        {
            set { _title.Value = value; }
            get { return _title.Value; }
        }

        public float Value
        {
            set { _value.Value = value; }
            get { return _value.Value; }
        }

        public float MaxValue
        {
            set { _maxValue.Value = value; }
            get { return _maxValue.Value; }
        }

        public float MinValue
        {
            set { _minValue.Value = value; }
            get { return _minValue.Value; }
        }

        private void Awake()
        {
            // ReactiveProperty
            _maxValue.Subscribe(value => _slider.maxValue = value).AddTo(gameObject);
            _minValue.Subscribe(value => _slider.minValue = value).AddTo(gameObject);
            _title.Subscribe(value =>
            {
                gameObject.name = value;
                _text.text = value;
            }).AddTo(gameObject);
            _value.Subscribe(value =>
            {
                _slider.value = value;
                _inputField.text = value.ToString();
            }).AddTo(gameObject);

            // Unity UI
            _slider.OnValueChangedAsObservable().Subscribe(value => _value.Value = value).AddTo(gameObject);
            _inputField.OnEndEditAsObservable().Subscribe(text =>
            {
                float value = 0.0f;
                float.TryParse(text, out value);
                _value.Value = value;
            }).AddTo(gameObject);
        }
    }
}