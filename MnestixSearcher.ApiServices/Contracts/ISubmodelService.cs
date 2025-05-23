using AasCore.Aas3_0;
using Newtonsoft.Json.Linq;

namespace MnestixSearcher.ApiServices.Contracts
{
    public interface ISubmodelService
    {
        public Task<ICollection<Submodel>> GetSubmodelsAsync();
        public Task<Submodel> GetSubmodeById(string submodelId);
        public Task<Submodel> GetSubmodelMetadada(string submodelId);
    }
}
