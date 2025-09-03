using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.CustomBehaviours
{
    [UsedImplicitly]
    public class GraphicBehaviour : MonoBehaviour
    {
        [UsedImplicitly]
        public Graphic Graphic => graphic = graphic ? graphic : GetComponent<Graphic>();
        protected Graphic graphic;
    }
}