using System;

namespace CustomUtils.Runtime.StartUp
{
    internal static class StepFactory
    {
        internal static StepBase CreateStep(Type stepType)
        {
            if (typeof(StepBase).IsAssignableFrom(stepType) is false)
                throw new ArgumentException($"Type {stepType.Name} does not derive from BaseStep");

            return Activator.CreateInstance(stepType) as StepBase;
        }
    }
}