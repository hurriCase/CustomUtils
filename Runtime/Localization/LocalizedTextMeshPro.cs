using CustomUtils.Runtime.CustomBehaviours;
using R3;
using TMPro;
using UnityEngine;

namespace CustomUtils.Runtime.Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    internal sealed class LocalizedTextMeshPro : TextBehaviour
    {
        [field: SerializeField] internal string LocalizationKey { get; set; }

        private TMP_FontAsset _originalFont;

        private void Awake()
        {
            _originalFont = Text.font;
        }

        private void Start()
        {
            Localize();

            LocalizationController.Language.Subscribe(this, (_, textMeshPro) => textMeshPro.Localize())
                .RegisterTo(destroyCancellationToken);
        }

        private void Localize()
        {
            Text.text = LocalizationController.Localize(LocalizationKey);

            var isFontSpecified =
                LocalizationController.TryGetFontForLanguage(LocalizationController.Language.Value,
                    out var languageFontMapping);

            Text.font = isFontSpecified ? languageFontMapping.Font : _originalFont;
        }
    }
}