using CustomUtils.Runtime.Attributes;
using UnityEngine;

namespace CustomUtils.Runtime.UI
{
    [RequireComponent(typeof(RectTransform))]
    internal sealed class SafeArea : MonoBehaviour
    {
        [SerializeField, Self] private RectTransform _rectTransform;

        [SerializeField] private bool _verticalSymmetry;
        [SerializeField] private bool _horizontalSymmetry;

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

            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;

            var offsetMax = new Vector2(Screen.width, Screen.height) - (safeArea.size + safeArea.position);

            if (_horizontalSymmetry)
                UpdateHorizontalSymmetry(anchorMin, offsetMax, ref safeArea);

            if (_verticalSymmetry)
                UpdateVerticalSymmetry(anchorMin, offsetMax, ref safeArea);

            UpdateAnchors(anchorMin, safeArea);
        }

        private void UpdateHorizontalSymmetry(Vector2 anchorMin, Vector2 offsetMax, ref Rect safeArea)
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

        private void UpdateVerticalSymmetry(Vector2 anchorMin, Vector2 offsetMax, ref Rect safeArea)
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

        private void UpdateAnchors(Vector2 anchorMin, Rect safeArea)
        {
            var anchorMax = anchorMin + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;
        }
    }
}