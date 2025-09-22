using CustomUtils.Runtime.Attributes;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Extensions.Observables;
using TMPro;
using UnityEngine;

namespace CustomUtils.Runtime.Localization
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    internal sealed class LocalizedTextMeshPro : MonoBehaviour
    {
        [field: SerializeField] internal string LocalizationKey { get; set; }

        [SerializeField, Self] private TextMeshProUGUI _text;

        private void Start()
        {
            Localize();

            LocalizationController.Language.SubscribeUntilDestroy(this, self => self.Localize());
        }

        private void Localize()
        {
            if (LocalizationKey.IsValid() is false)
            {
                Debug.LogWarning($"[LocalizedTextMeshPro::Localize] Localization key is invalid: {LocalizationKey}");
                return;
            }

            _text.text = LocalizationController.Localize(LocalizationKey);
        }
    }
}