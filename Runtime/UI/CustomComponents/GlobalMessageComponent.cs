using CustomUtils.Runtime.CustomBehaviours;
using JetBrains.Annotations;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.CustomComponents
{
    [UsedImplicitly]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class GlobalMessageComponent : CanvasGroupBehaviour
    {
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Button _button;

        [SerializeField] private float _showingDuration = 2f;
        [SerializeField] private float _fadingDuration = 0.5f;
        [SerializeField] private bool _shouldAutoClose = true;

        private Tween _tween;

        private void Awake()
        {
            if (_button)
                _button.onClick.AddListener(() =>
                {
                    _tween.Stop();
                    HideMessage();
                });

            CanvasGroup.alpha = 0f;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
        }

        [UsedImplicitly]
        public void ShowMessage(string message)
        {
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;

            _messageText.SetText(message);

            _tween.Stop();

            _tween = Tween.Alpha(
                CanvasGroup,
                endValue: 1f,
                _fadingDuration,
                useUnscaledTime: true,
                endDelay: _showingDuration
            ).OnComplete(this, globalMessage =>
            {
                globalMessage.CanvasGroup.alpha = 1f;

                if (globalMessage._shouldAutoClose)
                    globalMessage.HideMessage();
            });
        }

        [UsedImplicitly]
        public void HideMessage()
        {
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;

            _tween.Stop();

            _tween = Tween.Alpha(
                CanvasGroup,
                endValue: 0f,
                _fadingDuration,
                useUnscaledTime: true
            ).OnComplete(this, globalMessage => globalMessage.CanvasGroup.alpha = 0f);
        }

        private void OnDisable()
        {
            _tween.Stop();
        }
    }
}