using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace CustomUtils.Runtime.CustomBehaviours
{
    [UsedImplicitly]
    public class TextBehaviour : MonoBehaviour
    {
        [UsedImplicitly]
        public TextMeshProUGUI Text => _text = _text ? _text : GetComponent<TextMeshProUGUI>();
        private TextMeshProUGUI _text;
    }
}