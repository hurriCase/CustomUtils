using CustomUtils.Runtime.Attributes;
using CustomUtils.Runtime.Extensions.Observables;
using TMPro;
using UnityEngine;

namespace CustomUtils.Runtime.Localization
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    internal sealed class LocalizedTextMeshPro : MonoBehaviour
    {
        [field: SerializeField] internal string LocalizationKey { get; private set; }

        [field: SerializeField, Self] internal TextMeshProUGUI Text { get; private set; }

        private void Start()
        {
            LocalizationController.Language.SubscribeUntilDestroy(this, self => self.Localize());
        }

        private void Localize()
        {
            if (string.IsNullOrEmpty(LocalizationKey))
            {
                Debug.LogWarning("[LocalizedTextMeshPro::Localize] Localization key is invalid", gameObject);
                return;
            }

            Text.text = LocalizationController.Localize(LocalizationKey);
        }
    }
}