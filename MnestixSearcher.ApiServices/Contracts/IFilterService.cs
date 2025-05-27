using AasCore.Aas3_0;
using MnestixSearcher.AasSearcher;
using MnestixSearcher.ApiServices.Dto;

namespace MnestixSearcher.ApiServices.Contracts
{
    public interface IFilterService
    {
        public Task<SubmodelSemanticResult> FilterSubmodelsBySemanticKey(IReference reference);
        public Task HandleSubmodel(string submodelId, AasSearchEntry record);
        public Task<string> HandleTechnicalData(string submodelId);
    }
}
