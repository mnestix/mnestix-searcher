using Microsoft.Extensions.Options;
using MnestixSearcher.ApiClient;
using MnestixSearcher.ApiServices.Contracts;
using MnestixSearcher.ApiServices.Contracts.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MnestixSearcher.AasSearcher;

public class AasSearcherService
{
    private readonly IMongoCollection<AasSearchEntry> _aasSearchEntries;
    private readonly IAasService _aasService;

    public AasSearcherService(
        IOptions<AasSearchDatabaseSettings> aasSearchDatabaseSettings,
        IAasService assetAdministrationShellRepositoryAPIApi)
    {
        _aasService = assetAdministrationShellRepositoryAPIApi;
        
        var mongoClient = new MongoClient(
            aasSearchDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            aasSearchDatabaseSettings.Value.DatabaseName);

        _aasSearchEntries = mongoDatabase.GetCollection<AasSearchEntry>(
            aasSearchDatabaseSettings.Value.CollectionName);
        
        InitializeCollectionAsync().Wait();

    }
    
    public async Task<List<AasSearchEntry>> GetAsync() =>
        await _aasSearchEntries.Find(_ => true).ToListAsync();
    
    public async Task<List<AasSearchEntry>> GetByCriteriaAsync(FilterDefinition<AasSearchEntry> filter) =>
        await _aasSearchEntries.Find(filter).ToListAsync();
    public async Task FillDatabase()
    {
      
        try
        {
            var shells = await _aasService.GetAssetAdministrationShellsAsync();

            Console.WriteLine(apiResponse.ToString());
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Exception during API call: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
        

        // Delete all existing documents in the collection
        await _aasSearchEntries.DeleteManyAsync(_ => true);

        // Fallback to sample data if API fails
        var aasSearchEntries = new List<AasSearchEntry>
        {
            new AasSearchEntry
            {
                Id = "aasId1",
                ProductRoot = "ProductRoot1",
                ProductFamily = "ProductFamily1",
                ProductDesignation = "ProductDesignation1",
                ProductClassifications = new Dictionary<string, object>
                {
                    {"ECLASS", new {Version = "1.2.3", ProductId = "safsafdsf"}},
                    {"VEC", new {Version = "1.2.3", ProductId = "safsafdsf"}},
                },
             /*   TechnicalProperties = new Dictionary<string, object>
                { // always use the full idShortPath as key 
                    { "TechnicalData.TechnicalProperties.Size", "10" },
                    { "TechnicalData.TechnicalProperties.Family", new { länge = 10, breite = 5, höhe = 2 } }
                } */
            },
            new AasSearchEntry
            {
                Id = "aadId2",
                ProductRoot = "ProductRoot2",
                ProductFamily = "ProductFamily2",
                ProductDesignation = "ProductDesignation2",
                ProductClassifications = new Dictionary<string, object> {
                    {"ECLASS", new {Version = "1.2.3", ProductId = "safsafdsf"}},
                    {"VEC", new {Version = "1.2.3", ProductId = "safsafdsf"}},
                },
              /*  TechnicalProperties = new Dictionary<string, object>
                { // always use the full idShortPath as key 
                    { "TechnicalData.TechnicalProperties.Size", "100" },
                    { "TechnicalData.TechnicalProperties.Family", new { länge = 10, breite = 5, höhe = 2 }}
                } */
            }
        };

        await _aasSearchEntries.InsertManyAsync(aasSearchEntries);
    }
    private async Task InitializeCollectionAsync()
    {
        try
        {
            // Check if collection is already there
            var filter = new BsonDocument("name", _aasSearchEntries.CollectionNamespace.CollectionName);
            var collections = await _aasSearchEntries.Database.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
            
            if (!await collections.AnyAsync())
            {
                await _aasSearchEntries.Database.CreateCollectionAsync(_aasSearchEntries.CollectionNamespace.CollectionName);
                
                var indexKeysDefinition = Builders<AasSearchEntry>.IndexKeys
                    .Ascending(entry => entry.ProductRoot)
                    .Ascending(entry => entry.ProductFamily)
                    .Ascending(entry => entry.ProductDesignation);
                
                await _aasSearchEntries.Indexes.CreateOneAsync(new CreateIndexModel<AasSearchEntry>(indexKeysDefinition));
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error initializing Collection", ex);
        }
    }
}