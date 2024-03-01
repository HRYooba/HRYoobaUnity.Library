using UnityEngine;

namespace HRYooba.UI
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public class OperationCanvas : MonoBehaviour
    {
        [SerializeField] private float _showAlpha = 1.0f;
        [SerializeField] private float _hideAlpha = 0.0f;

        private CanvasGroup _canvasGroup = null;

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

        public void SetActive(bool value)
        {
            _canvasGroup.alpha = value ? _showAlpha : _hideAlpha;
            _canvasGroup.blocksRaycasts = value;
            _canvasGroup.interactable = value;
        }
    }
}