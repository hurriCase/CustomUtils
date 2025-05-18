using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CustomUtils.Runtime.StartUp
{
    // ReSharper disable once MemberCanBeInternal
    /// <summary>
    /// Base class for initialization steps in the application startup process.
    /// </summary>
    public abstract class StepBase
    {
        /// <summary>
        /// Event that is triggered when the step completes execution.
        /// </summary>
        /// <remarks>
        /// The first parameter is the step index, and the second parameter is the step name.
        /// </remarks>
        public event Action<int, string> OnStepCompleted;

        internal virtual async UniTask Execute(int step)
        {
            try
            {
                await ExecuteInternal();
                OnStepCompleted?.Invoke(step, GetType().Name);
            }
            catch (Exception e)
            {
                Debug.LogError($"[{GetType().Name}::Execute] Step initialization failed: {e.Message}");
            }
        }

        /// <summary>
        /// Contains the core implementation for the specific step.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected abstract UniTask ExecuteInternal();
    }
}