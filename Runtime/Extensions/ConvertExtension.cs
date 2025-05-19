using System.Collections;
using System.Threading.Tasks;
using JetBrains.Annotations;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace CustomUtils.Runtime.Extensions
{
    [UsedImplicitly]
    public static class ConvertExtension
    {
        /// <summary>
        /// Converts a Task into an IEnumerator that can be used with Unity's Coroutine system.
        /// Yields null each frame until the task completes. Throws any exceptions from the task.
        /// </summary>
        /// <param name="task">The Task to wait for completion.</param>
        /// <returns>An IEnumerator that completes when the task completes.</returns>
        /// <exception cref="System.AggregateException">Thrown when the task fails with an exception.</exception>
        [UsedImplicitly]
        public static IEnumerator WaitForTask(this Task task)
        {
            while (task.IsCompleted is false)
            {
                yield return null;
            }

            if (task.Exception != null)
                throw task.Exception;
        }
    }
}