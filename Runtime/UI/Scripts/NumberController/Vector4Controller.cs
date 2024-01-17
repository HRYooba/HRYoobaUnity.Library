using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace HRYooba.UI
{
    [ExecuteInEditMode]
    public class Vector4Controller : MonoBehaviour
    {
        [Header("Value")]
        [SerializeField] private StringReactiveProperty _title = new StringReactiveProperty("Vector4Controller");
        [SerializeField] private Vector4ReactiveProperty _value = new Vector4ReactiveProperty(Vector4.zero);
        [SerializeField] private Vector4ReactiveProperty _maxValue = new Vector4ReactiveProperty(Vector4.one);
        [SerializeField] private Vector4ReactiveProperty _minValue = new Vector4ReactiveProperty(Vector4.zero);

        [Header("UI")]
        [SerializeField] private Slider[] _sliders = new Slider[4];
        [SerializeField] private InputField[] _inputFields = new InputField[4];
        [SerializeField] private Text[] _texts = new Text[4];

        public System.IObservable<Vector4> OnValueChanged
        {
            get { return _value; }
        }

        public string Title
        {
            set { _title.Value = value; }
            get { return _title.Value; }
        }

        public Vector4 Value
        {
            set { _value.Value = value; }
            get { return _value.Value; }
        }

        public Vector4 MaxValue
        {
            set { _maxValue.Value = value; }
            get { return _maxValue.Value; }
        }

        public Vector4 MinValue
        {
            set { _minValue.Value = value; }
            get { return _minValue.Value; }
        }

        private void Awake()
        {
            // ReactiveProperty
            _maxValue.Subscribe(value =>
            {
                _sliders[0].maxValue = value.x;
                _sliders[1].maxValue = value.y;
                _sliders[2].maxValue = value.z;
                _sliders[3].maxValue = value.w;
            }).AddTo(gameObject);
            _minValue.Subscribe(value =>
            {
                _sliders[0].minValue = value.x;
                _sliders[1].minValue = value.y;
                _sliders[2].minValue = value.z;
                _sliders[3].minValue = value.w;
            }).AddTo(gameObject);
            _title.Subscribe(value =>
            {
                gameObject.name = value;
                _texts[0].text = value + ".X";
                _texts[1].text = value + ".Y";
                _texts[2].text = value + ".Z";
                _texts[3].text = value + ".W";
            }).AddTo(gameObject);
            _value.Subscribe(value =>
            {
                _sliders[0].value = value.x;
                _sliders[1].value = value.y;
                _sliders[2].value = value.z;
                _sliders[3].value = value.w;
                _inputFields[0].text = value.x.ToString();
                _inputFields[1].text = value.y.ToString();
                _inputFields[2].text = value.z.ToString();
                _inputFields[3].text = value.w.ToString();
            }).AddTo(gameObject);

            // Unity UI
            _sliders[0].OnValueChangedAsObservable().Subscribe(value => _value.Value = new Vector4(value, _value.Value.y, _value.Value.z, _value.Value.w)).AddTo(gameObject);
            _sliders[1].OnValueChangedAsObservable().Subscribe(value => _value.Value = new Vector4(_value.Value.x, value, _value.Value.z, _value.Value.w)).AddTo(gameObject);
            _sliders[2].OnValueChangedAsObservable().Subscribe(value => _value.Value = new Vector4(_value.Value.x, _value.Value.y, value, _value.Value.w)).AddTo(gameObject);
            _sliders[3].OnValueChangedAsObservable().Subscribe(value => _value.Value = new Vector4(_value.Value.x, _value.Value.y, _value.Value.z, value)).AddTo(gameObject);
            _inputFields[0].OnEndEditAsObservable().Subscribe(text =>
            {
                float value = 0.0f;
                float.TryParse(text, out value);
                _value.Value = new Vector4(value, _value.Value.y, _value.Value.z);
            }).AddTo(gameObject);
            _inputFields[1].OnEndEditAsObservable().Subscribe(text =>
            {
                float value = 0.0f;
                float.TryParse(text, out value);
                _value.Value = new Vector4(_value.Value.x, value, _value.Value.z);
            }).AddTo(gameObject);
            _inputFields[2].OnEndEditAsObservable().Subscribe(text =>
            {
                float value = 0.0f;
                float.TryParse(text, out value);
                _value.Value = new Vector4(_value.Value.x, _value.Value.y, value);
            }).AddTo(gameObject);
            _inputFields[3].OnEndEditAsObservable().Subscribe(text =>
            {
                float value = 0.0f;
                float.TryParse(text, out value);
                _value.Value = new Vector4(_value.Value.x, _value.Value.y, value);
            }).AddTo(gameObject);
        }
    }
}