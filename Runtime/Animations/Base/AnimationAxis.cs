﻿using JetBrains.Annotations;

namespace CustomUtils.Runtime.Animations.Base
{
    /// <summary>
    /// Defines which axis or axes to animate.
    /// </summary>
    [UsedImplicitly]
    public enum AnimationAxis
    {
        None = 0,
        X = 1,
        Y = 2,
        Both = 3
    }
}