using MnestixSearcher.AasSearcher;
using MongoDB.Driver;

namespace MnestixSearcher.ApiServices.Contracts
{
    public interface ISearchService
    {
        Task<List<AasSearchEntry>> GetAsync();

        Task<List<AasSearchEntry>> GetByCriteriaAsync(FilterDefinition<AasSearchEntry> filter);
    }
}
