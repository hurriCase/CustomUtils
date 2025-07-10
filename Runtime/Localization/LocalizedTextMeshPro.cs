using CustomUtils.Runtime.CustomBehaviours;
using R3;
using TMPro;
using UnityEngine;

namespace CustomUtils.Runtime.Localization
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    internal sealed class LocalizedTextMeshPro : TextBehaviour
    {
        [field: SerializeField] internal string LocalizationKey { get; set; }

        private void Start()
        {
            Localize();

            LocalizationController.Language.Subscribe(this, (_, textMeshPro) => textMeshPro.Localize())
                .RegisterTo(destroyCancellationToken);
        }

        private void Localize() => Text.text = LocalizationController.Localize(LocalizationKey);
    }
}