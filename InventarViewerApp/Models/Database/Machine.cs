namespace InventarViewerApp.Models.Database;

public record Machine
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? OperatingSystem { get; set; }
    public DateTime? LastSeen { get; set; }
    public DateTime CreatedAt { get; set; }
}