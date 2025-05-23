using AasCore.Aas3_0;
using Microsoft.Extensions.Options;
using MnestixSearcher.ApiServices.Contracts;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Cryptography.Xml;

namespace MnestixSearcher.AasSearcher;

public class AasSearcherService
{
    private readonly IMongoCollection<AasSearchEntry> _aasSearchEntries;
    private readonly IAasService _aasService;
    private readonly ISubmodelService _submodelService;

    public AasSearcherService(
        IOptions<AasSearchDatabaseSettings> aasSearchDatabaseSettings,
        IAasService aasService,
        ISubmodelService submodelService)
    {
        _aasService = aasService;
        _submodelService = submodelService;

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

            foreach (var shell in shells)
            {
                if (shell?.Submodels == null)
                    continue;

                foreach (var submodelRef in shell.Submodels)
                {
                    var reference = submodelRef?.Keys?.FirstOrDefault(key => key.Type == KeyTypes.Submodel);
                    var submodelId = reference?.Value;
                    if (string.IsNullOrWhiteSpace(submodelId))
                        continue;

                    var metadata = await _submodelService.GetSubmodelMetadada(submodelId);
                    var semanticKeyValue = metadata?.SemanticId?.Keys?
                        .FirstOrDefault(key => key.Type == KeyTypes.GlobalReference || key.Type == KeyTypes.Submodel)
                        ?.Value?.ToLower();

                    if (string.IsNullOrEmpty(semanticKeyValue))
                        continue;

                    if (semanticKeyValue.Contains("nameplate"))
                    {
                        var submodel = await _submodelService.GetSubmodeById(submodelId);
                        var elements = submodel?.SubmodelElements;

                        var productRoot = elements?
                            .Where(el => SemanticIdContains(el, "0173-1#02-AAU732#001"))
                            .ToList();

                        if (productRoot?.Count == 1)
                        {
                            var value = ((MultiLanguageProperty)productRoot[0])?.Value?[0].Text.ToString();
                            Console.WriteLine(value);
                        }
                    }
                    else if (semanticKeyValue.Contains("technicaldata"))
                    {
                        var submodel = await _submodelService.GetSubmodeById(submodelId);
                        Console.WriteLine(submodel);
                    }
                }

            }
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

    private static bool SemanticIdContains(ISubmodelElement el, string keyword)
    {
        return el.SemanticId?.Keys?
            .Any(key => key.Type == KeyTypes.GlobalReference &&
                        key.Value.Contains(keyword, StringComparison.OrdinalIgnoreCase)) == true;
    }

}