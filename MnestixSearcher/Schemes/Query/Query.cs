using MnestixSearcher.AasSearcher;
using MnestixSearcher.ApiServices.Contracts;
using HotChocolate.Data;

namespace MnestixSearcher.Schemes.Query
{
    public class Query
    {
        [UseFiltering]
        public async Task<List<AasSearchEntry>> GetEntries([Service] IAasSearcherService aasSearcherService)
        {
            return await aasSearcherService.GetAsync();
        }
    }
}
