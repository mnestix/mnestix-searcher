using AasCore.Aas3_0;
using MnestixSearcher.ApiServices.Contracts;
using MnestixSearcher.ApiServices.Contracts.Repository;
using System.Text.Json.Nodes;

namespace MnestixSearcher.ApiServices.Services
{
    public class AasService(IAasRepoService aasRepoService) : IAasService
    {
        private readonly IAasRepoService _aasRepoService = aasRepoService;

        public async Task<ICollection<AssetAdministrationShell>> GetAssetAdministrationShellsAsync()
        {
            List<AssetAdministrationShell> assetAdministrationShells = [];
            var response = await _aasRepoService.GetAssetAdministrationShell();
            JsonNode? responseObject = JsonNode.Parse(response)?["result"];

            if (responseObject != null)
            {
                foreach (var aas in responseObject.AsArray())
                {
                    if (aas == null) continue;
                    assetAdministrationShells.Add(Jsonization.Deserialize.AssetAdministrationShellFrom(aas));
                }
            }
            return assetAdministrationShells;
        }
    }
}
