using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MnestixSearcher.AasSearcher;

public class AasSearcherService
{
    private readonly IMongoCollection<AasSearchEntry> _aasSearchEntries;

    public AasSearcherService(IOptions<AasSearchDatabaseSettings> aasSearchDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            aasSearchDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            aasSearchDatabaseSettings.Value.DatabaseName);

        _aasSearchEntries = mongoDatabase.GetCollection<AasSearchEntry>(
            aasSearchDatabaseSettings.Value.CollectionName);

    }
    
    public async Task<List<AasSearchEntry>> GetAsync() =>
        await _aasSearchEntries.Find(_ => true).ToListAsync();
    
    public async Task<List<AasSearchEntry>> GetByCriteriaAsync(FilterDefinition<AasSearchEntry> filter) =>
        await _aasSearchEntries.Find(filter).ToListAsync();
    
    public async Task FillDatabase()
    {
        // Delete all existing documents in the collection
        await _aasSearchEntries.DeleteManyAsync(_ => true);
        
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

}