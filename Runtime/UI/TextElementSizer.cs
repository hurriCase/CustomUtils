using TMPro;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Runtime.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    [ExecuteInEditMode]
    internal sealed class TextElementSizer : MonoBehaviour
    {
        private TextMeshProUGUI _textComponent;
        private RectTransform _rectTransform;
        private RectTransform _parentRectTransform;

        private string _lastText = string.Empty;
        private Vector2 _lastParentSize = Vector2.zero;

        private void OnEnable()
        {
            _rectTransform = GetComponent<RectTransform>();
            _textComponent = GetComponent<TextMeshProUGUI>();
            _parentRectTransform = transform.parent ? transform.parent.GetComponent<RectTransform>() : null;

            if (_parentRectTransform)
                _lastParentSize = _parentRectTransform.rect.size;

#if UNITY_EDITOR
            EditorApplication.delayCall += OptimizeTextSize;
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.delayCall -= OptimizeTextSize;
#endif
        }

        private void Update()
        {
            var needsUpdate = _textComponent.text != _lastText;

            if (_parentRectTransform)
            {
                var currentParentSize = _parentRectTransform.rect.size;
                if (currentParentSize != _lastParentSize)
                {
                    _lastParentSize = currentParentSize;
                    needsUpdate = true;
                }
            }

            if (needsUpdate)
                OptimizeTextSize();
        }

        private void OptimizeTextSize()
        {
            if (!_parentRectTransform || string.IsNullOrEmpty(_textComponent.text))
                return;

            _lastText = _textComponent.text;

            _rectTransform.sizeDelta = _parentRectTransform.rect.size;

            _textComponent.ForceMeshUpdate(true);

            var textWidth = _textComponent.textBounds.size.x;
            textWidth = Mathf.Min(textWidth, _parentRectTransform.rect.width);
            _rectTransform.sizeDelta = new Vector2(textWidth, _rectTransform.sizeDelta.y);
        }
    }
}