using UnityEngine;
using UnityEngine.UI;
using R3;

namespace HRYooba.UI
{
    [ExecuteAlways]
    public class IntController : MonoBehaviour
    {
        [SerializeField] private bool _isInfinity = false;

        [Header("Value")]
        [SerializeField] private SerializableReactiveProperty<string> _title = new("IntController");
        [SerializeField] private SerializableReactiveProperty<int> _value = new(0);
        [SerializeField] private SerializableReactiveProperty<int> _minValue = new(0);
        [SerializeField] private SerializableReactiveProperty<int> _maxValue = new(10);

        [Header("UI")]
        [SerializeField] private Slider _slider = null;
        [SerializeField] private InputField _inputField = null;
        [SerializeField] private Text _text = null;

        public Observable<int> OnValueChangedObservable => _value;

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

        public int Value
        {
            set => _value.Value = value;
            get => _value.Value;
        }

        public int MinValue
        {
            set => _minValue.Value = value;
            get => _minValue.Value;
        }

        public int MaxValue
        {
            set => _maxValue.Value = value;
            get => _maxValue.Value;
        }

        private void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif

            // ReactiveProperty
            _value.Subscribe(OnValueChanged).AddTo(gameObject);
            _minValue.Subscribe(OnMinValueChanged).AddTo(gameObject);
            _maxValue.Subscribe(OnMaxValueChanged).AddTo(gameObject);
            _title.Subscribe(OnTitleChanged).AddTo(gameObject);

            // Unity UI
            _slider.OnValueChangedAsObservable().Subscribe(OnSliderValueChanged).AddTo(gameObject);
            _inputField.OnEndEditAsObservable().Subscribe(OnInputFieldEndEdit).AddTo(gameObject);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying) return;
            OnValueChanged(_value.Value);
            OnMinValueChanged(_minValue.Value);
            OnMaxValueChanged(_maxValue.Value);
            OnTitleChanged(_title.Value);
        }
#endif

        private void OnValueChanged(int value)
        {
            _slider.value = _isInfinity ? (_minValue.Value + _maxValue.Value) / 2.0f : value;
            _inputField.text = value.ToString();
        }

        private void OnMinValueChanged(int value)
        {
            if (!_isInfinity) _slider.minValue = value;
        }

        private void OnMaxValueChanged(int value)
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
                _value.Value += Mathf.RoundToInt(diff);
            }
            else
            {
                _value.Value = Mathf.RoundToInt(value);
            }
        }

        private void OnInputFieldEndEdit(string text)
        {
            int.TryParse(text, out var value);
            _value.Value = value;
        }
    }
}