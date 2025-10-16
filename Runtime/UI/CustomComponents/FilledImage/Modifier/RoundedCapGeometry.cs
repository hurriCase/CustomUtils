using CustomUtils.Runtime.Extensions;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.FilledImage.Modifier
{
    internal sealed class RoundedCapGeometry : CapGeometryBase
    {
        internal override Vector2[] CreateStartCap(CapParameters parameters, float startRadians) =>
            CreateCap(parameters, startRadians, -Mathf.PI);

        internal override Vector2[] CreateEndCap(CapParameters parameters, float endRadians) =>
            CreateCap(parameters, endRadians, Mathf.PI);

        private Vector2[] CreateCap(CapParameters parameters, float radians, float initialOffset)
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

            return points;
        }
    }
}