using Microsoft.Extensions.Options;
using MnestixSearcher.ApiServices.Contracts.Repository;
using MnestixSearcher.ApiServices.Settings;
using MnestixSearcher.ApiServices.Types;
using RestSharp;

namespace MnestixSearcher.ApiServices.HttpClient
{
    public class HttpRepoClient : IAasHttpClient, ISubmodelHttpClient, IConceptHttpClient
    {
        private RestClient _restClient;

        public HttpRepoClient(string baseUrlSettings)
        {
            var options = new RestClientOptions(baseUrlSettings);
            _restClient = new RestClient(options);
        }

        public async Task<RestResponse> GetAsync(string url)
        {
            var request = new RestRequest(url);
            return await _restClient.ExecuteAsync(request);
        }
    }
}
