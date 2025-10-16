using CustomUtils.Runtime.Extensions;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.FilledImage
{
    internal readonly struct CapGeometry
    {
        internal Vector2[] CapPoints { get; }
        internal bool HasCap => CapPoints?.Length > 0;

        private CapGeometry(Vector2[] points)
        {
            CapPoints = points;
        }

        internal static CapGeometry CreateStartCap(bool isEnabled, CapParameters parameters, float startRadians) =>
            isEnabled
                ? CreateCap(parameters, startRadians, -Mathf.PI)
                : default;

        internal static CapGeometry CreateEndCap(bool isEnabled, CapParameters parameters, float endRadians) =>
            isEnabled
                ? CreateCap(parameters, endRadians, Mathf.PI)
                : default;

        private static CapGeometry CreateCap(CapParameters parameters, float radians, float initialOffset)
        {
            var points = new Vector2[parameters.Resolution + 1];
            var capRadius = (parameters.OuterRadius - parameters.InnerRadius) * 0.5f;
            var middleRadius = parameters.InnerRadius + capRadius;
            var direction = radians.GetDirectionFromAngle();
            var centerPoint = parameters.Center + direction * middleRadius;
            var capAngleStart = radians + initialOffset;

            for (var i = 0; i <= parameters.Resolution; i++)
            {
                var t = (float)i / parameters.Resolution;
                var angle = Mathf.Lerp(capAngleStart, radians, t);
                points[i] = centerPoint + angle.GetDirectionFromAngle() * capRadius;
            }

            return new CapGeometry(points);
        }
    }
}