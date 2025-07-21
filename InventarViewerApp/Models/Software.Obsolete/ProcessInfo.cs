namespace InventarViewerApp.Models.Software;

public record ProcessInfo
{
    public string ProcessName { get; set; } = string.Empty;
    public int ProcessId { get; set; }
    public string MainWindowTitle { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public long WorkingSet { get; set; }
    public long VirtualMemorySize { get; set; }
    public TimeSpan TotalProcessorTime { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileVersion { get; set; } = string.Empty;
    public string ProductVersion { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsResponding { get; set; }
    public int ThreadCount { get; set; }
}
