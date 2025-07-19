using InventarViewerApp.Models.Software;
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

}