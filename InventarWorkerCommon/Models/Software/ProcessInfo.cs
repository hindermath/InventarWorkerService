namespace InventarWorkerCommon.Models.Software;

/// <summary>
/// Represents information about a running or installed process on the system.
/// </summary>
public record ProcessInfo
{
    /// <summary>
    /// Gets or sets the process name (executable name without path).
    /// </summary>
    public string ProcessName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the process identifier (PID).
    /// </summary>
    public int ProcessId { get; set; }

    /// <summary>
    /// Gets or sets the title of the process's main window, if any.
    /// </summary>
    public string MainWindowTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the time when the process was started.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Gets or sets the current working set (physical memory) used by the process, in bytes.
    /// </summary>
    public long WorkingSet { get; set; }

    /// <summary>
    /// Gets or sets the virtual memory size of the process, in bytes.
    /// </summary>
    public long VirtualMemorySize { get; set; }

    /// <summary>
    /// Gets or sets the total processor time consumed by the process.
    /// </summary>
    public TimeSpan TotalProcessorTime { get; set; }

    /// <summary>
    /// Gets or sets the full path to the executable file for the process.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file version of the process executable.
    /// </summary>
    public string FileVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product version of the process executable.
    /// </summary>
    public string ProductVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company name associated with the executable.
    /// </summary>
    public string Company { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a descriptive text for the process or executable.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the process is responding to user input.
    /// </summary>
    public bool IsResponding { get; set; }

    /// <summary>
    /// Gets or sets the number of threads currently running in the process.
    /// </summary>
    public int ThreadCount { get; set; }
}
