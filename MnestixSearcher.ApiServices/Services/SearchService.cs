using Microsoft.Extensions.Options;
using MnestixSearcher.AasSearcher;
using MnestixSearcher.ApiServices.Contracts;
using MnestixSearcher.ApiServices.Settings;
using MongoDB.Driver;

namespace MnestixSearcher.ApiServices.Services
{
    public class SearchService : ISearchService
    {
        private readonly IMongoCollection<AasSearchEntry> _aasSearchEntries;

        public SearchService(
            IOptions<AasSearchDatabaseSettings> aasSearchDatabaseSettings)
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

        public async Task<List<AasSearchEntry>> GetByCriteriaAsync(FilterDefinition<AasSearchEntry> filter)
        {
            return await _aasSearchEntries.Find(filter).ToListAsync();
        }
    }
}
