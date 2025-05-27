using MnestixSearcher.AasSearcher;

namespace MnestixSearcher.ApiServices.Contracts
{
    public interface IAasSearcherService
    {
        Task<List<AasSearchEntry>> GetAsync();
    }
}