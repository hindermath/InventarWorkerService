using InventarWorkerCommon.Models.Software;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InventarViewerApp.Services;

public class MongoDbService
{
    private readonly string _connectionString;
    private readonly IMongoClient _mongoClient;
    private IMongoDatabase _mongoDatabase;

    public MongoDbService(string connectionString)
    {
        _connectionString = connectionString;
        _mongoClient = new MongoClient(_connectionString);
    }

    public void InitializeMongoDatabase()
    {
        _mongoDatabase = _mongoClient.GetDatabase("Inventory");
    }

    public async Task SaveSoftwareInventoryAsync(int machineId, SoftwareInventory software)
    {
        var collection = _mongoDatabase.GetCollection<BsonDocument>($"{machineId}");
        await collection.InsertOneAsync(software.ToBsonDocument());
    }

    // Verwendung der MongoDB Builder
    public async Task<List<BsonDocument>> FindSoftwareByNameAsync(int machineId, string softwareName)
    {
        var collection = _mongoDatabase.GetCollection<BsonDocument>($"{machineId}");
        
        // Filter Builder verwenden
        var filter = Builders<BsonDocument>.Filter.Eq("InstalledSoftware.Name", softwareName);
        
        // Projection Builder verwenden
        var projection = Builders<BsonDocument>.Projection
            .Include("InstalledSoftware.Name")
            .Include("InstalledSoftware.Version")
            .Exclude("_id");
        
        return await collection.Find(filter)
            .Project(projection)
            .ToListAsync();
    }

}