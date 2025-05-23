using MnestixSearcher.ApiServices.Types;
using RestSharp;

namespace MnestixSearcher.ApiServices.Contracts.Repository
{
    public interface IHttpRepoClient
    {
        public Task<RestResponse> GetAsync(string url);
    }

    public interface IAasHttpClient : IHttpRepoClient { }
    public interface ISubmodelHttpClient : IHttpRepoClient { }
    public interface IConceptHttpClient : IHttpRepoClient { }
}
