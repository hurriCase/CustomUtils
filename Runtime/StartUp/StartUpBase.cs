using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.StartUp
{
    /// <summary>
    /// Base class for managing the application startup process.
    /// </summary>
    public abstract class StartUpBase
    {
        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Gets a value indicating whether the initialization process has completed.
        /// </summary>
        public static bool IsInited { get; private set; }

        /// <summary>
        /// Event that is triggered when the entire initialization process completes successfully.
        /// </summary>
        public static event Action OnInitializationCompleted;

        private static readonly List<Type> _stepTypesList = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticMembers()
        {
            OnInitializationCompleted = null;
            IsInited = false;
        }

        /// <summary>
        /// Registers initialization steps to be executed during application startup.
        /// </summary>
        /// <param name="steps">Types of steps to register. All types must derive from StepBase.</param>
        /// <remarks>
        /// Steps will be executed in the order they're registered.
        /// </remarks>
        [UsedImplicitly]
        public static void RegisterStepTypes(params Type[] steps)
        {
            foreach (var step in steps)
            {
                if (typeof(StepBase).IsAssignableFrom(step))
                    _stepTypesList.Add(step);
                else
                    Debug.LogError($"[StartUpBase::RegisterSteps] Type {step.Name} does not derive from StepBase");
            }
        }

        /// <summary>
        /// Begins the application initialization process.
        /// </summary>
        /// <remarks>
        /// This method executes all registered initialization steps in sequence.
        /// When all steps complete successfully, the <see cref="OnInitializationCompleted"/> event is triggered.
        /// If the application is already initialized, this method does nothing.
        /// </remarks>
        [UsedImplicitly]
        public static async void InitializeApplication()
        {
            try
            {
                if (IsInited)
                    return;

                for (var i = 0; i < _stepTypesList.Count; i++)
                {
                    var step = StepFactory.CreateStep(_stepTypesList[i]);
                    step.OnStepCompleted += LogStepCompletion;
                    await step.Execute(i);
                }

                IsInited = true;
                OnInitializationCompleted?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"[StartUpController::InitializeApplication] Initialization failed, with error: {e.Message}");
            }
        }

        private static void LogStepCompletion(int step, string stepName)
        {
            Debug.Log($"[StartUpController::LogStepCompletion] Step {step} completed: {stepName}");
        }
    }
}