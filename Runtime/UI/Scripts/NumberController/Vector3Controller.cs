using UnityEngine;
using R3;

namespace HRYooba.UI
{
    [ExecuteAlways]
    public class Vector3Controller : MonoBehaviour
    {
        [SerializeField] private bool _isInfinity = false;

        [Header("Value")]
        [SerializeField] private SerializableReactiveProperty<string> _title = new("Vector3Controller");
        [SerializeField] private SerializableReactiveProperty<Vector3> _value = new(Vector3.zero);
        [SerializeField] private SerializableReactiveProperty<Vector3> _minValue = new(Vector3.zero);
        [SerializeField] private SerializableReactiveProperty<Vector3> _maxValue = new(Vector3.one);

        [Header("UI")]
        [SerializeField] private FloatController _x = null;
        [SerializeField] private FloatController _y = null;
        [SerializeField] private FloatController _z = null;

        public Observable<Vector3> OnValueChangedObservable => _value;

        public bool IsInfinity
        {
            set
            {
                _isInfinity = value;
                _x.IsInfinity = value;
                _y.IsInfinity = value;
                _z.IsInfinity = value;
            }
            get => _isInfinity;
        }

        public string Title
        {
            set => _title.Value = value;
            get => _title.Value;
        }

        public Vector3 Value
        {
            set => _value.Value = value;
            get => _value.Value;
        }

        public Vector3 MinValue
        {
            set => _minValue.Value = value;
            get => _minValue.Value;
        }

        public Vector3 MaxValue
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
            _z.OnValueChangedObservable.Subscribe(OnZValueChanged).AddTo(gameObject);
        }

        private void Update()
        {
            _x.IsInfinity = _isInfinity;
            _y.IsInfinity = _isInfinity;
            _z.IsInfinity = _isInfinity;

#if UNITY_EDITOR
            if (Application.isPlaying) return;
            OnValueChanged(_value.Value);
            OnMinValueChanged(_minValue.Value);
            OnMaxValueChanged(_maxValue.Value);
            OnTitleChanged(_title.Value);
#endif
        }

        private void OnValueChanged(Vector3 value)
        {
            _x.Value = value.x;
            _y.Value = value.y;
            _z.Value = value.z;
        }

        private void OnMinValueChanged(Vector3 value)
        {
            _x.MinValue = value.x;
            _y.MinValue = value.y;
            _z.MinValue = value.z;
        }

        private void OnMaxValueChanged(Vector3 value)
        {
            _x.MaxValue = value.x;
            _y.MaxValue = value.y;
            _z.MaxValue = value.z;
        }

        private void OnTitleChanged(string value)
        {
            gameObject.name = value;
            _x.Title = value + ".x";
            _y.Title = value + ".y";
            _z.Title = value + ".z";
        }

        private void OnXValueChanged(float value)
        {
            _value.Value = new Vector3(value, _value.Value.y, _value.Value.z);
        }

        private void OnYValueChanged(float value)
        {
            _value.Value = new Vector3(_value.Value.x, value, _value.Value.z);
        }

        private void OnZValueChanged(float value)
        {
            _value.Value = new Vector3(_value.Value.x, _value.Value.y, value);
        }
    }
}