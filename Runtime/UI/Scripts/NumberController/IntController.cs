using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace HRYooba.UI
{
    [ExecuteInEditMode]
    public class IntController : MonoBehaviour
    {
        [Header("Value")]
        [SerializeField] private StringReactiveProperty _title = new StringReactiveProperty("IntController");
        [SerializeField] private IntReactiveProperty _value = new IntReactiveProperty(0);
        [SerializeField] private IntReactiveProperty _maxValue = new IntReactiveProperty(10);
        [SerializeField] private IntReactiveProperty _minValue = new IntReactiveProperty(0);

        [Header("UI")]
        [SerializeField] private Slider _slider = null;
        [SerializeField] private InputField _inputField = null;
        [SerializeField] private Text _text = null;

        public System.IObservable<int> OnValueChanged
        {
            get { return _value; }
        }

        public string Title
        {
            set { _title.Value = value; }
            get { return _title.Value; }
        }

        public int Value
        {
            set { _value.Value = value; }
            get { return _value.Value; }
        }

        public int MaxValue
        {
            set { _maxValue.Value = value; }
            get { return _maxValue.Value; }
        }

        public int MinValue
        {
            set { _minValue.Value = value; }
            get { return _minValue.Value; }
        }

        private void Awake()
        {
            // ReactiveProperty
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
            _maxValue.Subscribe(value => _slider.maxValue = value).AddTo(gameObject);
            _minValue.Subscribe(value => _slider.minValue = value).AddTo(gameObject);

            // Unity UI
            _slider.OnValueChangedAsObservable().Subscribe(value => _value.Value = (int)value).AddTo(gameObject);
            _inputField.OnEndEditAsObservable().Subscribe(text =>
            {
                int value = 0;
                int.TryParse(text, out value);
                _value.Value = value;
            }).AddTo(gameObject);
        }
    }
}