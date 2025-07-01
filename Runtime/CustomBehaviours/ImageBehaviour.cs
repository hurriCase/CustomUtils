using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.CustomBehaviours
{
    [UsedImplicitly]
    public class ImageBehaviour : MonoBehaviour
    {
        [UsedImplicitly]
        public Image Image => _image = _image ? _image : GetComponent<Image>();
        private Image _image;
    }
}