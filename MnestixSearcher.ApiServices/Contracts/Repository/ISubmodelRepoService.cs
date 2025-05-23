using MnestixSearcher.ApiServices.Types;

namespace MnestixSearcher.ApiServices.Contracts.Repository
{
    public interface ISubmodelRepoService
    {
        public Task<string> GetSubmodels();
        public Task<string> GetSubmodelInformation(string submodelId, SubmodelInfoType type);
    }
}
