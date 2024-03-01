using UnityEngine;
using UnityEngine.UI;
using R3;

namespace HRYooba.UI
{
    [ExecuteAlways]
    public class FloatController : MonoBehaviour
    {
        [SerializeField] private bool _isInfinity = false;

        [Header("Value")]
        [SerializeField] private SerializableReactiveProperty<string> _title = new("FloatController");
        [SerializeField] private SerializableReactiveProperty<float> _value = new(0.0f);
        [SerializeField] private SerializableReactiveProperty<float> _minValue = new(0.0f);
        [SerializeField] private SerializableReactiveProperty<float> _maxValue = new(1.0f);

        [Header("UI")]
        [SerializeField] private Slider _slider = null;
        [SerializeField] private InputField _inputField = null;
        [SerializeField] private Text _text = null;

        public Observable<float> OnValueChangedObservable => _value;

        public bool IsInfinity
        {
            set => _isInfinity = value;
            get => _isInfinity;
        }

        public string Title
        {
            set => _title.Value = value;
            get => _title.Value;
        }

        public float Value
        {
            set => _value.Value = value;
            get => _value.Value;
        }

        public float MinValue
        {
            set => _minValue.Value = value;
            get => _minValue.Value;
        }

        public float MaxValue
        {
            set => _maxValue.Value = value;
            get => _maxValue.Value;
        }

        private void Awake()
        {
            if (!Application.isPlaying) return;

            // ReactiveProperty
            _value.Subscribe(OnValueChanged).AddTo(gameObject);
            _minValue.Subscribe(OnMinValueChanged).AddTo(gameObject);
            _maxValue.Subscribe(OnMaxValueChanged).AddTo(gameObject);
            _title.Subscribe(OnTitleChanged).AddTo(gameObject);

            // Unity UI
            _slider.OnValueChangedAsObservable().Subscribe(OnSliderValueChanged).AddTo(gameObject);
            _inputField.OnEndEditAsObservable().Subscribe(OnInputFieldEndEdit).AddTo(gameObject);
        }

        private void Update()
        {
            if (Application.isPlaying) return;
            OnValueChanged(_value.Value);
            OnMinValueChanged(_minValue.Value);
            OnMaxValueChanged(_maxValue.Value);
            OnTitleChanged(_title.Value);
        }

        private void OnValueChanged(float value)
        {
            _slider.value = _isInfinity ? (_minValue.Value + _maxValue.Value) / 2.0f : value;
            _inputField.text = value.ToString();
        }

        private void OnMinValueChanged(float value)
        {
            if (!_isInfinity) _slider.minValue = value;
        }

        private void OnMaxValueChanged(float value)
        {
            if (!_isInfinity) _slider.maxValue = value;
        }

        private void OnTitleChanged(string value)
        {
            gameObject.name = value;
            _text.text = value;
        }

        private void OnSliderValueChanged(float value)
        {
            if (_isInfinity)
            {
                var center = (_maxValue.Value + _minValue.Value) / 2.0f;
                var diff = value - center;
                _value.Value += diff;
            }
            else
            {
                _value.Value = value;
            }
        }

        private void OnInputFieldEndEdit(string text)
        {
            float.TryParse(text, out var value);
            _value.Value = value;
        }
    }
}