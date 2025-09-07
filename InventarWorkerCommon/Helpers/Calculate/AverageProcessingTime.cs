namespace InventarWorkerCommon.Helpers.Calculate;

/// <summary>
/// Provides helper methods to calculate processing-time related metrics.
/// </summary>
public static class AverageProcessingTime
{
    /// <summary>
    /// Calculates the average processing time per item in milliseconds since the given start time.
    /// </summary>
    /// <param name="processedItems">The number of items processed.</param>
    /// <param name="startTime">The time when processing started.</param>
    /// <returns>The average processing time per item in milliseconds, or 0 if no items were processed.</returns>
    public static double CalculateAverageProcessingTime(int processedItems, DateTime startTime)
    {
        return processedItems > 0 ? (DateTime.Now - startTime).TotalMilliseconds / processedItems : 0;
    }
}