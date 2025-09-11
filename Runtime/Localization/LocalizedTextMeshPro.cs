using CustomUtils.Runtime.CustomBehaviours;
using CustomUtils.Runtime.Extensions;
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

            LocalizationController.Language.SubscribeAndRegister(this, self => self.Localize());
        }

        private void Localize()
        {
            Text.text = LocalizationController.Localize(LocalizationKey);
        }
    }
}