using InventarWorkerCommon.Models.Software;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InventarWorkerCommon.Services.Database;

/// <summary>
/// Provides basic MongoDB operations for persisting and querying software inventory documents.
/// </summary>
public class MongoDbService
{
    private readonly string _connectionString;
    private readonly IMongoClient _mongoClient;
    private IMongoDatabase _mongoDatabase;

    /// <summary>
    /// Initializes a new instance of MongoDbService with the given connection string.
    /// </summary>
    /// <param name="connectionString">The MongoDB connection string.</param>
    public MongoDbService(string connectionString)
    {
        _connectionString = connectionString;
        _mongoClient = new MongoClient(_connectionString);
    }

    /// <summary>
    /// Initializes the MongoDB database used by this service.
    /// </summary>
    public void InitializeMongoDatabase()
    {
        _mongoDatabase = _mongoClient.GetDatabase("Inventory");
    }

    /// <summary>
    /// Saves a software inventory document for the specified machine ID.
    /// </summary>
    /// <param name="machineId">The target machine ID used as the collection name.</param>
    /// <param name="software">The software inventory to persist.</param>
    public async Task SaveSoftwareInventoryAsync(int machineId, SoftwareInventory software)
    {
        var collection = _mongoDatabase.GetCollection<BsonDocument>($"{machineId}");
        await collection.InsertOneAsync(software.ToBsonDocument());
    }

    /// <summary>
    /// Finds software entries by name for the specified machine ID.
    /// </summary>
    /// <param name="machineId">The machine ID collection to query.</param>
    /// <param name="softwareName">The software name to match.</param>
    /// <returns>A list of BSON documents containing matching software entries.</returns>
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