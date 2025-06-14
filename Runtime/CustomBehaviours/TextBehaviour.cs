using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace CustomUtils.Runtime.CustomBehaviours
{
    [UsedImplicitly]
    public class TextBehaviour : MonoBehaviour
    {
        [UsedImplicitly]
        public TextMeshProUGUI TextComponent =>
            _textComponent = _textComponent ? _textComponent : GetComponent<TextMeshProUGUI>();
        private TextMeshProUGUI _textComponent;
    }
}