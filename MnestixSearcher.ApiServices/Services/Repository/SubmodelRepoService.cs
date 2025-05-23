using MnestixSearcher.ApiServices.Contracts;
using MnestixSearcher.ApiServices.Contracts.Repository;
using MnestixSearcher.ApiServices.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnestixSearcher.ApiServices.Services.Repository
{
    public class SubmodelRepoService(ISubmodelHttpClient submodelHttpClient, IBase64Service base64Service) : ISubmodelRepoService
    {
        private readonly ISubmodelHttpClient _submodelHttpClient = submodelHttpClient;
        private readonly IBase64Service _base64Service = base64Service;

        public async Task<string> GetSubmodelInformation(string submodelId, SubmodelInfoType type)
        {
            string encodedSubmodelId = _base64Service.Base64Encode(submodelId);
            var response = new RestSharp.RestResponse();

            switch (type)
            {
                case SubmodelInfoType.Submodel:
                    response = await _submodelHttpClient.GetAsync($"/submodels/{encodedSubmodelId}");
                    break;
                case SubmodelInfoType.SubElements:
                    response = await _submodelHttpClient.GetAsync($"/submodels/{encodedSubmodelId}/submodel-elements");
                    break;
            }

            if (response.IsSuccessful)
            {
                return response.Content != null ? response.Content : $"No Content returned {response.StatusCode}";
            }
            else
            {
                return response.ErrorMessage != null ? response.ErrorMessage : $"Error with status code: {response.StatusCode}";
            }
        }

        public async Task<string> GetSubmodels()
        {
            var response = await _submodelHttpClient.GetAsync("/submodels");
            if (response.IsSuccessful)
            {
                return response.Content != null ? response.Content : $"No Content returned {response.StatusCode}";
            }
            else
            {
                return response.ErrorMessage != null ? response.ErrorMessage : $"Error with status code: {response.StatusCode}";
            }
        }
    }
}
