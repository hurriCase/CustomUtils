﻿using TMPro;
using UnityEngine;

namespace CustomUtils.Runtime.Localization
{
    /// <summary>
    /// Localize text component with font support.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    internal sealed class LocalizedTextMeshPro : MonoBehaviour
    {
        [field: SerializeField] internal string LocalizationKey { get; private set; }

        private TextMeshProUGUI _textComponent;
        private TMP_FontAsset _originalFont;

        private void Awake()
        {
            _textComponent = GetComponent<TextMeshProUGUI>();

            _originalFont = _textComponent.font;
        }

        private void Start()
        {
            Localize();
            LocalizationManager.OnLocalizationChanged += Localize;
        }

        private void OnDestroy()
        {
            LocalizationManager.OnLocalizationChanged -= Localize;
        }

        private void Localize()
        {
            _textComponent ??= GetComponent<TextMeshProUGUI>();

            _textComponent.text = LocalizationManager.Localize(LocalizationKey);

            var isFontSpecified =
                LocalizationManager.TryGetFontForLanguage(LocalizationManager.Language, out var languageFontMapping);

            _textComponent.font = isFontSpecified ? languageFontMapping.Font : _originalFont;
        }

        public void ResetFont()
        {
            _textComponent ??= GetComponent<TextMeshProUGUI>();

            _textComponent.font = _originalFont;
        }
    }
}