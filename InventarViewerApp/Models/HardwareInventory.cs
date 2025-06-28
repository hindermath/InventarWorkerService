namespace InventarViewerApp.Models;

public class HardwareInventory
{
    public int Id { get; set; }
    public string MachineName { get; set; }
    public string Processor { get; set; }
    public string Memory { get; set; }
    public string DiskSpace { get; set; }
    public string NetworkInfo { get; set; }
    public DateTime Timestamp { get; set; }
}