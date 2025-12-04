namespace InventarWorkerCommon.Helpers.Calculate;

/// <summary>
/// Provides an extension method for calculating the average processing time per item.
/// </summary>
public static class AverageProcessingTimeExtension
{
    /// <summary>
    /// Calculates the average processing time per item based on the total number of processed items
    /// and the start time of the operation.
    /// </summary>
    /// <param name="processedItems">
    /// The total number of items that have been processed.
    /// </param>
    /// <param name="startTime">
    /// The start time of the operation used to calculate the processing duration.
    /// </param>
    /// <returns>
    /// The average processing time per item, in milliseconds. Returns 0 if no items have been processed.
    /// </returns>
    public static double CalculateAverageProcessingTime(this int processedItems, DateTime startTime)
    {
        return processedItems > 0 ? (DateTime.Now - startTime).TotalMilliseconds / processedItems : 0;
    }
}