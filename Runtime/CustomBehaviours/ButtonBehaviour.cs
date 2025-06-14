using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.CustomBehaviours
{
    [UsedImplicitly]
    public class ButtonBehaviour : MonoBehaviour
    {
        [UsedImplicitly]
        public Button TextComponent =>
            _buttonComponent = _buttonComponent ? _buttonComponent : GetComponent<Button>();
        private Button _buttonComponent;
    }
}