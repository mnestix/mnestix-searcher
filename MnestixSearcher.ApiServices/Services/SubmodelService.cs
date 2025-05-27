using AasCore.Aas3_0;
using Microsoft.Extensions.Logging;
using MnestixSearcher.ApiServices.Contracts;
using MnestixSearcher.ApiServices.Contracts.Repository;
using MnestixSearcher.ApiServices.Types;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace MnestixSearcher.ApiServices.Services
{
    public class SubmodelService(ISubmodelRepoService submodelRepoService, ILogger<SubmodelService> logger) : ISubmodelService
    {
        private readonly ISubmodelRepoService _submodelRepoService = submodelRepoService;
        private readonly ILogger<SubmodelService> _logger = logger;

        public async Task<Submodel> GetSubmodeById(string submodelId)
        {
            var response = await _submodelRepoService.GetSubmodelInformation(submodelId, SubmodelInfoType.Submodel);
            try
            {
                JsonNode? responseObject = JsonNode.Parse(response);
                var submodel = Jsonization.Deserialize.SubmodelFrom(responseObject);

                return submodel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return null;
        }

        public async Task<Submodel?> GetSubmodelMetadada(string submodelId)
        {
            var response = await _submodelRepoService.GetSubmodelInformation(submodelId, SubmodelInfoType.MetaData);
            try
            {
                JsonNode? responseObject = JsonNode.Parse(response);
                var submodel = Jsonization.Deserialize.SubmodelFrom(responseObject);

                return submodel;
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return null;
        }

        public async Task<ICollection<Submodel>> GetSubmodelsAsync()
        {
            List<Submodel> submodels = [];
            var response = await _submodelRepoService.GetSubmodels();
            JsonNode? responseObject = JsonNode.Parse(response)?["result"];

            if (responseObject != null)
            {
                foreach (var submodel in responseObject.AsArray())
                {
                    if (submodel == null) continue;
                    submodels.Add(Jsonization.Deserialize.SubmodelFrom(submodel));
                }
            }
            return submodels;
        }
    }
}
