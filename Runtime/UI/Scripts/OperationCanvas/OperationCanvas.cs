using UnityEngine;

namespace HRYooba.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public class OperationCanvas : MonoBehaviour
    {
        [SerializeField] private bool _isActive = true;
        [SerializeField, Range(0.0f, 1.0f)] private float _alpha = 1.0f;

        private CanvasGroup _canvasGroup = null;

        public bool IsActive => _isActive;

        public float Alpha
        {
            get => _alpha;
            set
            {
                _alpha = value;
                _canvasGroup.alpha = _alpha;
            }
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying) return;

            SetActive(_isActive);
        }
#endif

        public void SetActive(bool value)
        {
            _isActive = value;
            _canvasGroup.alpha = value ? _alpha : 0.0f;
            _canvasGroup.blocksRaycasts = value;
            _canvasGroup.interactable = value;
        }
    }
}