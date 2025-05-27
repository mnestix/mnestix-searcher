using Microsoft.Extensions.Options;
using MnestixSearcher.AasSearcher;
using MnestixSearcher.ApiServices.Contracts;
using MnestixSearcher.ApiServices.Settings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MnestixSearcher.ApiServices.Services;

public class AasSearcherService : IAasSearcherService
{
    private readonly IMongoCollection<AasSearchEntry> _aasSearchEntries;
    private readonly IAasService _aasService;
    private readonly IFilterService _filterService;

    public AasSearcherService(
        IOptions<AasSearchDatabaseSettings> aasSearchDatabaseSettings,
        IAasService aasService,
        IFilterService filterService)
    {
        _aasService = aasService;
        _filterService = filterService;
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

    public async Task<List<AasSearchEntry>> GetByCriteriaAsync(FilterDefinition<AasSearchEntry> filter)
    {
        return await _aasSearchEntries.Find(filter).ToListAsync();
    }

    public async Task FillDatabase()
    {

        try
        {
            List<AasSearchEntry> store = [];
            var shells = await _aasService.GetAssetAdministrationShellsAsync();

            foreach (var shell in shells)
            {
                if (shell?.Submodels == null || shell.AssetInformation.AssetKind != AasCore.Aas3_0.AssetKind.Type)
                    continue;

                var record = new AasSearchEntry { Id = shell.Id, CreatedTime = DateTime.UtcNow, ThumbnailUrl = shell.AssetInformation.DefaultThumbnail?.Path };

                foreach (var submodelRef in shell.Submodels)
                {
                    var smIdResult = await _filterService.FilterSubmodelsBySemanticKey(submodelRef);

                    if (!smIdResult.Success)
                    {
                        Console.WriteLine($"Error: {smIdResult.ErrorMessage}");
                        continue;
                    }

                    if (smIdResult.SemanticKeyValue.Contains("nameplate") || smIdResult.SemanticKeyValue.Contains("technicaldata"))
                    {
                        await _filterService.HandleSubmodel(smIdResult.SubmodelId, record);
                    }
                }

                if (record.SaveData) store.Add(record);
            }

            // Delete all existing documents in the collection
            await _aasSearchEntries.DeleteManyAsync(_ => true);

            await _aasSearchEntries.InsertManyAsync(store);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during API call: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
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
                    .Ascending(entry => entry.Id)
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