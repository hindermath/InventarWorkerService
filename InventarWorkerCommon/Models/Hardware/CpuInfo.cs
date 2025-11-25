namespace InventarWorkerCommon.Models.Hardware;

/// <summary>
/// Describes CPU-related information such as core count, name, architecture and current usage.
/// </summary>
public record CpuInfo
{
    /// <summary>
    /// Gets or sets the number of logical processors available on the machine.
    /// </summary>
    public int ProcessorCount { get; init; }

    /// <summary>
    /// Gets or sets the human-readable CPU name or model (for example, "Intel(R) Core(TM) i7").
    /// </summary>
    public string ProcessorName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the CPU architecture, such as x86, x64, or ARM.
    /// </summary>
    public string Architecture { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the current overall CPU usage in percent (0–100).
    /// </summary>
    public double CurrentUsage { get; set; }
}
