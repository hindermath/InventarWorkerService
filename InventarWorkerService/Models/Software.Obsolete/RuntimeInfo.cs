namespace InventarWorkerService.Models.Software;

public record RuntimeInfo
{
    public string DotNetVersion { get; set; } = string.Empty;
    public List<string> InstalledFrameworks { get; set; } = new();
    public string PowerShellVersion { get; set; } = string.Empty;
    public List<string> AvailableModules { get; set; } = new();
}
