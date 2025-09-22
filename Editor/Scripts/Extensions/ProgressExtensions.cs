using System;

namespace CustomUtils.Editor.Scripts.Extensions
{
    public static class ProgressExtensions
    {
        public static void UpdateProgress(this IProgress<float> progress, ref int processedCount, int totalCount)
        {
            processedCount++;
            progress.Report((float)processedCount / totalCount);
        }
    }
}