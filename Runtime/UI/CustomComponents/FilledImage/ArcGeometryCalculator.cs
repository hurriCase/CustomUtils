using CustomUtils.Runtime.Constants;
using CustomUtils.Runtime.Extensions;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.FilledImage
{
    internal sealed class ArcGeometryCalculator
    {
        private const float HalfPivot = 0.5f;
        private const int MinimumArcSegments = 6;
        private const float SegmentsPerRadian = 20f;
        private const float CornerRadiusRatio = 0.2f;
        private const float HalfCircleRadians = Mathf.PI * 0.5f;

        internal ArcGeometry Calculate(
            RectTransform rectTransform,
            float thicknessRatio,
            float customFillOrigin,
            float fillAmount,
            bool fillClockwise,
            bool hasRoundedCaps)
        {
            var rect = rectTransform.rect;
            var center = rect.center;
            var maxRadius = Mathf.Min(rect.width, rect.height) * HalfPivot;
            var thickness = maxRadius * thicknessRatio;
            var innerRadius = maxRadius - thickness;

            var (startRadians, endRadians) = CalculateFillAngles(customFillOrigin, fillAmount, fillClockwise);
            var arcLength = Mathf.Abs(endRadians - startRadians);
            var segmentCount = Mathf.Max(MinimumArcSegments, Mathf.FloorToInt(SegmentsPerRadian * arcLength));

            var (adjustedInner, adjustedOuter) = hasRoundedCaps
                ? (innerRadius, maxRadius)
                : (innerRadius + thickness * CornerRadiusRatio, maxRadius - thickness * CornerRadiusRatio);

            var innerPoints = new Vector2[segmentCount + 1];
            var outerPoints = new Vector2[segmentCount + 1];

            for (var i = 0; i <= segmentCount; i++)
            {
                var angle = Mathf.Lerp(startRadians, endRadians, (float)i / segmentCount);
                var direction = angle.GetDirectionFromAngle();

                innerPoints[i] = center + direction * adjustedInner;
                outerPoints[i] = center + direction * adjustedOuter;
            }

            var capGeometry = hasRoundedCaps
                ? CalculateCapGeometry(center, innerRadius, maxRadius, startRadians, endRadians, fillClockwise)
                : default;

            return new ArcGeometry(innerPoints, outerPoints, segmentCount, capGeometry);
        }

        private (float startRadians, float endRadians) CalculateFillAngles(
            float customFillOrigin,
            float fillAmount,
            bool fillClockwise)
        {
            var fullAngle = fillClockwise ? MathConstants.FullCircleDegrees : -MathConstants.FullCircleDegrees;
            var endAngle = customFillOrigin + fullAngle * fillAmount;
            return (customFillOrigin.ToRadians(), endAngle.ToRadians());
        }

        private CapGeometry CalculateCapGeometry(
            Vector2 center,
            float innerRadius,
            float outerRadius,
            float startRadians,
            float endRadians,
            bool fillClockwise)
        {
            var middleRadius = innerRadius + (outerRadius - innerRadius) * 0.5f;
            var capRadius = (outerRadius - innerRadius) * 0.5f;

            var startDirection = startRadians.GetDirectionFromAngle();
            var endDirection = endRadians.GetDirectionFromAngle();

            var angleOffset = fillClockwise ? HalfCircleRadians : -HalfCircleRadians;

            return new CapGeometry(
                center + startDirection * middleRadius,
                center + endDirection * middleRadius,
                capRadius,
                startRadians - angleOffset,
                endRadians + angleOffset
            );
        }
    }
}