using JetBrains.Annotations;

namespace CustomUtils.Runtime.StartUp
{
    /// <summary>
    /// Represents data for a completed initialization step.
    /// </summary>
    [PublicAPI]
    public readonly struct StepData
    {
        /// <summary>
        /// Gets the step number in the initialization sequence.
        /// </summary>
        public int Step { get; }

        /// <summary>
        /// Gets the name of the completed step.
        /// </summary>
        public string StepName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StepData"/> struct.
        /// </summary>
        /// <param name="step">The step number in the initialization sequence.</param>
        /// <param name="stepName">The name of the step.</param>
        public StepData(int step, string stepName)
        {
            Step = step;
            StepName = stepName;
        }
    }
}