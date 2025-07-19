using InventarViewerApp.Models.Software;

namespace InventarViewerApp.Services;

public class MongoDbService
{
    private readonly string _connectionString;

    public MongoDbService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void InitializeDatabase()
    {
        
    }

    public async Task SaveSoftwareInventoryAsync(int machineId, SoftwareInventory software)
    {
        
    }

}