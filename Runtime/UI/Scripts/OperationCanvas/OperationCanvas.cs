using UnityEngine;

namespace HRYooba.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public class OperationCanvas : MonoBehaviour
    {
        [SerializeField] private bool _isActive = true;
        [SerializeField] private float _showAlpha = 1.0f;
        [SerializeField] private float _hideAlpha = 0.0f;

        private CanvasGroup _canvasGroup = null;

        public bool IsActive => _isActive;

        public float ShowAlpha
        {
            set => _showAlpha = value;
            get => _showAlpha;
        }

        public float HideAlpha
        {
            set => _hideAlpha = value;
            get => _hideAlpha;
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
            _canvasGroup.alpha = value ? _showAlpha : _hideAlpha;
            _canvasGroup.blocksRaycasts = value;
            _canvasGroup.interactable = value;
        }
    }
}