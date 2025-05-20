using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MnestixSearcher.Clients.Api;
using MnestixSearcher.Clients.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MnestixSearcher.AasSearcher;

public class AasSearcherService
{
    private readonly IMongoCollection<AasSearchEntry> _aasSearchEntries;
    private readonly IAssetAdministrationShellRepositoryAPIApi _assetAdministrationShellRepositoryAPIApi;

    public AasSearcherService(
        IOptions<AasSearchDatabaseSettings> aasSearchDatabaseSettings,
        IAssetAdministrationShellRepositoryAPIApi assetAdministrationShellRepositoryAPIApi)
    {
        _assetAdministrationShellRepositoryAPIApi = assetAdministrationShellRepositoryAPIApi;
        
        var mongoClient = new MongoClient(
            aasSearchDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            aasSearchDatabaseSettings.Value.DatabaseName);

        _aasSearchEntries = mongoDatabase.GetCollection<AasSearchEntry>(
            aasSearchDatabaseSettings.Value.CollectionName);
        
    //    InitializeCollectionAsync().Wait();

    }
    
    public async Task<List<AasSearchEntry>> GetAsync() =>
        await _aasSearchEntries.Find(_ => true).ToListAsync();
    
    public async Task<List<AasSearchEntry>> GetByCriteriaAsync(FilterDefinition<AasSearchEntry> filter) =>
        await _aasSearchEntries.Find(filter).ToListAsync();
    public async Task FillDatabase()
    {
      
        try
        {
            var apiResponse = await _assetAdministrationShellRepositoryAPIApi.GetAllAssetAdministrationShellsAsync(limit: "1000");
            
            if (apiResponse.IsOk && apiResponse.TryOk(out var pagedResult))
            {
                Console.WriteLine($"API call succeeded. Checking response structure...");
                
                // The PagedResult contains result items in AdditionalProperties
                if (pagedResult.AdditionalProperties.TryGetValue("result", out var resultElement))
                {
                    // Deserialize the JSON element to a list of AAS
                    var aasItems = System.Text.Json.JsonSerializer.Deserialize<List<AssetAdministrationShell>>(
                        resultElement.GetRawText(), 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    if (aasItems != null && aasItems.Count > 0)
                    {
                        Console.WriteLine($"Found {aasItems.Count} Asset Administration Shells");
                        
                        // Process each AAS and add to database
                        foreach (var aas in aasItems)
                        {
                            Console.WriteLine($"Processing AAS: {aas.IdShort ?? aas.Id}");
                            // TODO: Convert AAS to AasSearchEntry and add to database
                        }
                    }
                    else
                    {
                        Console.WriteLine("No Asset Administration Shells found in the result");
                    }
                }
                else
                {
                    Console.WriteLine("Could not find 'result' property in the response");
                    // List all available properties in the response
                    Console.WriteLine("Available properties: " + string.Join(", ", pagedResult.AdditionalProperties.Keys));
                }
            }
            else
            {
                Console.WriteLine($"API call failed with status: {apiResponse.StatusCode}");
                if (apiResponse.IsBadRequest && apiResponse.TryBadRequest(out var errorResult))
                {
                    Console.WriteLine($"Bad request error: {errorResult?.Messages?.FirstOrDefault()?.Text}");
                }
            }        }
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