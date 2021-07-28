using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace HRYooba.UI
{
    [ExecuteInEditMode]
    public class Vector2Controller : MonoBehaviour
    {
        [Header("Value")]
        [SerializeField] private StringReactiveProperty _title = new StringReactiveProperty("Vector2Controller");
        [SerializeField] private Vector2ReactiveProperty _value = new Vector2ReactiveProperty(Vector2.zero);
        [SerializeField] private Vector2ReactiveProperty _maxValue = new Vector2ReactiveProperty(Vector2.one);
        [SerializeField] private Vector2ReactiveProperty _minValue = new Vector2ReactiveProperty(Vector2.zero);

        [Header("UI")]
        [SerializeField] private Slider[] _sliders = new Slider[2];
        [SerializeField] private InputField[] _inputFields = new InputField[2];
        [SerializeField] private Text[] _texts = new Text[2];

        public System.IObservable<Vector2> OnValueChanged
        {
            get { return _value; }
        }

        public string Title
        {
            set { _title.Value = value; }
            get { return _title.Value; }
        }

        public Vector2 Value
        {
            set { _value.Value = value; }
            get { return _value.Value; }
        }

        public Vector2 MaxValue
        {
            set { _maxValue.Value = value; }
            get { return _maxValue.Value; }
        }

        public Vector2 MinValue
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
                _texts[0].text = value + ".X";
                _texts[1].text = value + ".Y";
            }).AddTo(gameObject);
            _value.Subscribe(value =>
            {
                _sliders[0].value = value.x;
                _sliders[1].value = value.y;
                _inputFields[0].text = value.x.ToString();
                _inputFields[1].text = value.y.ToString();
            }).AddTo(gameObject);
            _maxValue.Subscribe(value =>
            {
                _sliders[0].maxValue = value.x;
                _sliders[1].maxValue = value.y;
            }).AddTo(gameObject);
            _minValue.Subscribe(value =>
            {
                _sliders[0].minValue = value.x;
                _sliders[1].minValue = value.y;
            }).AddTo(gameObject);

            // Unity UI
            _sliders[0].OnValueChangedAsObservable().Subscribe(value => _value.Value = new Vector2(value, _value.Value.y)).AddTo(gameObject);
            _sliders[1].OnValueChangedAsObservable().Subscribe(value => _value.Value = new Vector2(_value.Value.x, value)).AddTo(gameObject);
            _inputFields[0].OnEndEditAsObservable().Subscribe(text =>
            {
                float value = 0.0f;
                float.TryParse(text, out value);
                _value.Value = new Vector2(value, _value.Value.y);
            }).AddTo(gameObject);
            _inputFields[1].OnEndEditAsObservable().Subscribe(text =>
            {
                float value = 0.0f;
                float.TryParse(text, out value);
                _value.Value = new Vector2(_value.Value.x, value);
            }).AddTo(gameObject);
        }
    }
}