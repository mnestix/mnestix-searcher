using MnestixSearcher.AasSearcher;
using MongoDB.Driver;

namespace MnestixSearcher.ApiServices.Contracts
{
    public interface IAasSearcherService
    {
        Task<List<AasSearchEntry>> GetAsync();

        Task<List<AasSearchEntry>> GetByCriteriaAsync(FilterDefinition<AasSearchEntry> filter);

        Task FillDatabase();
    }
}