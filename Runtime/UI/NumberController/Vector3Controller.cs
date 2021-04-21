using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace HRYooba.UI
{
    [ExecuteInEditMode]
    public class Vector3Controller : MonoBehaviour
    {
        [Header("Value")]
        [SerializeField] private StringReactiveProperty _title = new StringReactiveProperty("Vector3Controller");
        [SerializeField] private Vector3ReactiveProperty _value = new Vector3ReactiveProperty(Vector3.zero);
        [SerializeField] private Vector3ReactiveProperty _maxValue = new Vector3ReactiveProperty(Vector3.one);
        [SerializeField] private Vector3ReactiveProperty _minValue = new Vector3ReactiveProperty(Vector3.zero);

        [Header("UI")]
        [SerializeField] private Slider[] _sliders = new Slider[3];
        [SerializeField] private InputField[] _inputFields = new InputField[3];
        [SerializeField] private Text[] _texts = new Text[3];

        public System.IObservable<Vector3> OnValueChanged
        {
            get { return _value; }
        }

        public string Title
        {
            set { _title.Value = value; }
            get { return _title.Value; }
        }

        public Vector3 Value
        {
            set { _value.Value = value; }
            get { return _value.Value; }
        }

        public Vector3 MaxValue
        {
            set { _maxValue.Value = value; }
            get { return _maxValue.Value; }
        }

        public Vector3 MinValue
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
                _texts[2].text = value + ".Z";
            }).AddTo(gameObject);
            _value.Subscribe(value =>
            {
                _sliders[0].value = value.x;
                _sliders[1].value = value.y;
                _sliders[2].value = value.z;
                _inputFields[0].text = value.x.ToString();
                _inputFields[1].text = value.y.ToString();
                _inputFields[2].text = value.z.ToString();
            }).AddTo(gameObject);
            _maxValue.Subscribe(value =>
            {
                _sliders[0].maxValue = value.x;
                _sliders[1].maxValue = value.y;
                _sliders[2].maxValue = value.z;
            }).AddTo(gameObject);
            _minValue.Subscribe(value =>
            {
                _sliders[0].minValue = value.x;
                _sliders[1].minValue = value.y;
                _sliders[2].minValue = value.z;
            }).AddTo(gameObject);

            // Unity UI
            _sliders[0].OnValueChangedAsObservable().Subscribe(value => _value.Value = new Vector3(value, _value.Value.y, _value.Value.z)).AddTo(gameObject);
            _sliders[1].OnValueChangedAsObservable().Subscribe(value => _value.Value = new Vector3(_value.Value.x, value, _value.Value.z)).AddTo(gameObject);
            _sliders[2].OnValueChangedAsObservable().Subscribe(value => _value.Value = new Vector3(_value.Value.x, _value.Value.y, value)).AddTo(gameObject);
            _inputFields[0].OnEndEditAsObservable().Subscribe(text =>
            {
                float value = 0.0f;
                float.TryParse(text, out value);
                _value.Value = new Vector3(value, _value.Value.y, _value.Value.z);
            }).AddTo(gameObject);
            _inputFields[1].OnEndEditAsObservable().Subscribe(text =>
            {
                float value = 0.0f;
                float.TryParse(text, out value);
                _value.Value = new Vector3(_value.Value.x, value, _value.Value.z);
            }).AddTo(gameObject);
            _inputFields[2].OnEndEditAsObservable().Subscribe(text =>
            {
                float value = 0.0f;
                float.TryParse(text, out value);
                _value.Value = new Vector3(_value.Value.x, _value.Value.y, value);
            }).AddTo(gameObject);
        }
    }
}