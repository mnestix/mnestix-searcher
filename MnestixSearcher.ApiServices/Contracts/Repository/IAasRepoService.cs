using MnestixSearcher.ApiServices.Types;

namespace MnestixSearcher.ApiServices.Contracts.Repository
{
    public interface IAasRepoService
    {
        public Task<string> GetAssetAdministrationShell(int? limit);
        public Task<string> GetAssetAdministrationShellInformation(string aasId, AasInfoType type);
    }
}
