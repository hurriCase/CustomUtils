using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.CustomBehaviours
{
    [UsedImplicitly]
    public class ButtonBehaviour : MonoBehaviour
    {
        [UsedImplicitly]
        public Button Button => _button = _button ? _button : GetComponent<Button>();
        private Button _button;
    }
}