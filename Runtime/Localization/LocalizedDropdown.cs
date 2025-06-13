using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.Localization
{
    /// <inheritdoc />
    /// <summary>
    /// Localize dropdown component.
    /// </summary>
    [RequireComponent(typeof(Dropdown))]
    internal sealed class LocalizedDropdown : MonoBehaviour
    {
        [field: SerializeField] internal string[] LocalizationKeys { get; private set; }

        internal void Start()
        {
            Localize();
            LocalizationController.OnLocalizationChanged += Localize;
        }

        internal void OnDestroy()
        {
            LocalizationController.OnLocalizationChanged -= Localize;
        }

        private void Localize()
        {
            var dropdown = GetComponent<Dropdown>();

            for (var i = 0; i < LocalizationKeys.Length; i++)
                dropdown.options[i].text = LocalizationController.Localize(LocalizationKeys[i]);

            if (dropdown.value < LocalizationKeys.Length)
                dropdown.captionText.text = LocalizationController.Localize(LocalizationKeys[dropdown.value]);
        }
    }
}