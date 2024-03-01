using UnityEngine;
using R3;

namespace HRYooba.UI
{
    [ExecuteAlways]
    public class Vector2Controller : MonoBehaviour
    {
        [SerializeField] private bool _isInfinity = false;

        [Header("Value")]
        [SerializeField] private SerializableReactiveProperty<string> _title = new("Vector2Controller");
        [SerializeField] private SerializableReactiveProperty<Vector2> _value = new(Vector2.zero);
        [SerializeField] private SerializableReactiveProperty<Vector2> _minValue = new(Vector2.zero);
        [SerializeField] private SerializableReactiveProperty<Vector2> _maxValue = new(Vector2.one);

        [Header("UI")]
        [SerializeField] private FloatController _x = null;
        [SerializeField] private FloatController _y = null;

        public Observable<Vector2> OnValueChangedObservable => _value;

        public bool IsInfinity
        {
            set
            {
                _isInfinity = value;
                _x.IsInfinity = value;
                _y.IsInfinity = value;
            }
            get => _isInfinity;
        }

        public string Title
        {
            set => _title.Value = value;
            get => _title.Value;
        }

        public Vector2 Value
        {
            set => _value.Value = value;
            get => _value.Value;
        }

        public Vector2 MinValue
        {
            set => _minValue.Value = value;
            get => _minValue.Value;
        }

        public Vector2 MaxValue
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
            _x.OnValueChangedObservable.Subscribe(OnXValueChanged).AddTo(gameObject);
            _y.OnValueChangedObservable.Subscribe(OnYValueChanged).AddTo(gameObject);
        }

        private void Update()
        {
            _x.IsInfinity = _isInfinity;
            _y.IsInfinity = _isInfinity;

#if UNITY_EDITOR
            if (Application.isPlaying) return;
            OnValueChanged(_value.Value);
            OnMinValueChanged(_minValue.Value);
            OnMaxValueChanged(_maxValue.Value);
            OnTitleChanged(_title.Value);
#endif
        }

        private void OnValueChanged(Vector2 value)
        {
            _x.Value = value.x;
            _y.Value = value.y;
        }

        private void OnMinValueChanged(Vector2 value)
        {
            _x.MinValue = value.x;
            _y.MinValue = value.y;
        }

        private void OnMaxValueChanged(Vector2 value)
        {
            _x.MaxValue = value.x;
            _y.MaxValue = value.y;
        }

        private void OnTitleChanged(string value)
        {
            gameObject.name = value;
            _x.Title = value + ".x";
            _y.Title = value + ".y";
        }

        private void OnXValueChanged(float value)
        {
            _value.Value = new Vector2(value, _value.Value.y);
        }

        private void OnYValueChanged(float value)
        {
            _value.Value = new Vector2(_value.Value.x, value);
        }
    }
}