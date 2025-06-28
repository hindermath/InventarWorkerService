namespace InventarViewerApp.Models;

public class SoftwareInventory
{
    public int Id { get; set; }
    public string OperatingSystem { get; set; }
    public string InstalledApplications { get; set; }
    public string Updates { get; set; }
    public DateTime Timestamp { get; set; }
}