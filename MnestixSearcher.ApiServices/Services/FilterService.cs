using AasCore.Aas3_0;
using MnestixSearcher.AasSearcher;
using MnestixSearcher.ApiServices.Contracts;
using MnestixSearcher.ApiServices.Dto;

namespace MnestixSearcher.ApiServices.Services
{
    internal class FilterService : IFilterService
    {
        private readonly ISubmodelService _submodelService;
        private readonly IVisitorFactory _visitorFactory;

        public FilterService(ISubmodelService submodelService, IVisitorFactory visitorFactory)
        {
            _submodelService = submodelService;
            _visitorFactory = visitorFactory;
        }

        public async Task<SubmodelSemanticResult> FilterSubmodelsBySemanticKey(IReference referenceInput)
        {
            var result = new SubmodelSemanticResult();

            var submodelId = referenceInput?.Keys?.FirstOrDefault(key => key.Type == KeyTypes.Submodel)?.Value;

            if (string.IsNullOrWhiteSpace(submodelId)){
                result.ErrorMessage = "Submodel Id not found";
                return result;
            };

            result.SubmodelId = submodelId;

            var metadata = await _submodelService.GetSubmodelMetadada(submodelId);

            var semanticKeyValue = metadata?.SemanticId?.Keys?
                .FirstOrDefault(key => key.Type == KeyTypes.GlobalReference || key.Type == KeyTypes.Submodel || key.Type == KeyTypes.ConceptDescription)
                ?.Value?.ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(semanticKeyValue))
            {
                result.ErrorMessage = "Semantic key not found or empty.";
                return result;
            }

            result.SemanticKeyValue = semanticKeyValue;
            result.Success = true;

            return result;
        }

        public async Task HandleSubmodel( string submodelId, AasSearchEntry record)
        {
            var submodel = await _submodelService.GetSubmodeById(submodelId);

            if (submodel == null)
            {
                Console.WriteLine("Submodel not found");
                return;
            };

            var visitor = _visitorFactory.Create(record);
            submodel.Accept(visitor);
        }

        public Task<string> HandleTechnicalData(string submodelId)
        {
            throw new NotImplementedException();
        }
    }
}
