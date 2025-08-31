namespace InventarWorkerCommon.Helpers.Calculate;

public static class AverageProcessingTime
{
    public static double CalculateAverageProcessingTime(int processedItems, DateTime startTime)
    {
        return processedItems > 0 ? (DateTime.Now - startTime).TotalMilliseconds / processedItems : 0;
    }
}