using MnestixSearcher.ApiServices.Contracts;
using MnestixSearcher.ApiServices.Contracts.Repository;
using MnestixSearcher.ApiServices.Types;

namespace MnestixSearcher.ApiServices.Services.Repository
{
    public class AasRepoService(IAasHttpClient aasHttpClient, IBase64Service base64Service) : IAasRepoService
    {
        private readonly IAasHttpClient _aasHttpClient = aasHttpClient;
        private readonly IBase64Service _base64Service = base64Service;

        public async Task<string> GetAssetAdministrationShell()
        {
            var response = await _aasHttpClient.GetAsync("/shells");
            if (response.IsSuccessful)
            {
                return response.Content != null ? response.Content : $"No Content returned {response.StatusCode}";
            }
            else
            {
                return response.ErrorMessage != null ? response.ErrorMessage : $"Error with status code: {response.StatusCode}";
            }
        }

        public async Task<string> GetAssetAdministrationShellInformation(string aasId, AasInfoType type)
        {
            string encodedAasId = _base64Service.Base64Encode(aasId);
            var response = new RestSharp.RestResponse();

            switch (type)
            {
                case AasInfoType.Aas:
                    response = await _aasHttpClient.GetAsync($"/shells/{encodedAasId}");
                    break;
                case AasInfoType.Asset:
                    response = await _aasHttpClient.GetAsync($"/shells/{encodedAasId}/asset-information");
                    break;
                case AasInfoType.Refs:
                    response = await _aasHttpClient.GetAsync($"/shells/{encodedAasId}/submodel-refs");
                    break;
                case AasInfoType.Thumbnail:
                    response = await _aasHttpClient.GetAsync($"/shells/{encodedAasId}/asset-information/thumbnail");
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
    }
}
