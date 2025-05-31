using UnityEngine;

namespace CustomUtils.Runtime.UI
{
    [RequireComponent(typeof(RectTransform))]
    internal sealed class SafeArea : MonoBehaviour
    {
        [SerializeField] private bool _verticalSymmetry = true;
        [SerializeField] private bool _horizontalSymmetry = true;

        private ScreenOrientation _lastOrientation;

        private void Awake()
        {
            Setup();
        }

        private void Update()
        {
            if (Screen.orientation != _lastOrientation)
                Setup();
        }

        private void Setup()
        {
            _lastOrientation = Screen.orientation;
            var rectTransform = GetComponent<RectTransform>();
            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;

            var offsetMax = new Vector2(Screen.width, Screen.height) - (safeArea.size + safeArea.position);
            if (_horizontalSymmetry)
            {
                if (anchorMin.x < offsetMax.x)
                {
                    anchorMin.x = offsetMax.x;
                    safeArea.size = new Vector2(safeArea.size.x - anchorMin.x, safeArea.size.y);
                }
                else
                {
                    if (anchorMin.x > offsetMax.x)
                        safeArea.size = new Vector2(safeArea.size.x - anchorMin.x, safeArea.size.y);
                }
            }

            if (_verticalSymmetry)
            {
                if (anchorMin.y < offsetMax.y)
                {
                    anchorMin.y = offsetMax.y;
                    safeArea.size = new Vector2(safeArea.size.x, safeArea.size.y - anchorMin.y);
                }
                else
                {
                    if (anchorMin.y > offsetMax.y)
                        safeArea.size = new Vector2(safeArea.size.x, safeArea.size.y - anchorMin.y);
                }
            }

            var anchorMax = anchorMin + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }
    }
}