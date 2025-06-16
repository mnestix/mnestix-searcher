using MnestixSearcher.AasSearcher;
using MnestixSearcher.ApiServices.Contracts;

namespace MnestixSearcher.Schemes.Query
{
    public class Query
    {
        [UseFiltering]
        public async Task<List<AasSearchEntry>> GetEntries([Service] ISearchService aasSearcherService)
        {
            return await aasSearcherService.GetAsync();
        }
    }
}
