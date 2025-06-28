namespace InventarWorkerService.Models.Hardware;

public record NetworkInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string NetworkInterfaceType { get; set; } = string.Empty;
    public string OperationalStatus { get; set; } = string.Empty;
    public long Speed { get; set; }
    public List<string> IpAddresses { get; set; } = new();
    public string MacAddress { get; set; } = string.Empty;
    public long BytesSent { get; set; }
    public long BytesReceived { get; set; }
}
