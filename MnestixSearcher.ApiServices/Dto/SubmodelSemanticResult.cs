namespace MnestixSearcher.ApiServices.Dto
{
    public class SubmodelSemanticResult
    {
        public string SubmodelId { get; set; } = string.Empty;
        public string SemanticKeyValue { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
