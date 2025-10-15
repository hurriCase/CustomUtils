using CustomUtils.Runtime.Extensions;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.FilledImage
{
    internal readonly struct CapGeometry
    {
        internal Vector2[] CapPoints { get; }
        internal bool HasCap => CapPoints?.Length > 0;

        private const float HalfCircleRadians = Mathf.PI * 0.5f;

        private CapGeometry(Vector2[] points)
        {
            CapPoints = points;
        }

        internal static CapGeometry CreateStartCap(bool isEnabled, CapParameters parameters, float radians) =>
            isEnabled
                ? CreateCap(parameters, radians, -HalfCircleRadians)
                : default;

        internal static CapGeometry CreateEndCap(bool isEnabled, CapParameters parameters, float radians) =>
            isEnabled
                ? CreateCap(parameters, radians, HalfCircleRadians)
                : default;

        private static CapGeometry CreateCap(CapParameters parameters, float radians, float initialOffset)
        {
            var points = new Vector2[parameters.Resolution + 1];
            var middleRadius = parameters.InnerRadius + (parameters.OuterRadius - parameters.InnerRadius) * 0.5f;
            var direction = radians.GetDirectionFromAngle();
            var centerPoint = parameters.Center + direction * middleRadius;
            var startAngle = radians + initialOffset;
            var capRadius = (parameters.OuterRadius - parameters.InnerRadius) * 0.5f;

            for (var i = 0; i <= parameters.Resolution; i++)
            {
                var t = (float)i / parameters.Resolution;
                var angle = startAngle + Mathf.Lerp(-HalfCircleRadians, HalfCircleRadians, t);
                points[i] = centerPoint + angle.GetDirectionFromAngle() * capRadius;
            }

            return new CapGeometry(points);
        }
    }
}