namespace MnestixSearcher.ApiServices.Types
{
    public static class SemanticGroups
    {
        public static readonly string[] ManufacturerName = {
            "0173-1#02-AAO677#004",
            "0173-1#02-AAO677#002"
        };

        public static readonly string[] ProductRoot = {
            "0173-1#02-AAU732#001",
            "0112/2///61360_7#AAS011#001"
        };

        public static readonly string[] ProductFamily = {
            "0173-1#02-AAU731#003",
            "0173-1#02-AAU731#001",
            "0112/2///61987#ABP464#002"
        };

        public static readonly string[] ProductDesignation = {
            "0173-1#02-AAW338#003",
            "0173-1#02-AAW338#001",
            "0112/2///61987#ABA567#009"
        };

        public static string[] AllMlProperties => [.. ProductRoot, .. ProductFamily, .. ProductDesignation, .. ManufacturerName];

        public static readonly string[] ProductClassificationItem = {
            "https://admin-shell.io/ZVEI/TechnicalData/ProductClassificationItem/1/1"
        };

        public static string[] AllSmElCollections => [.. ProductClassificationItem];

        public static string[] ProductClassificationSystem = {
            "https://admin-shell.io/ZVEI/TechnicalData/ProductClassificationSystem/1/1"
        };

        public static string[] ProductClassId = {
            "https://admin-shell.io/ZVEI/TechnicalData/ProductClassId/1/1"
        };

        public static string[] ClassificationSystemVersion = {
            "https://admin-shell.io/ZVEI/TechnicalData/ClassificationSystemVersion/1/1"
        };

        public static string[] AllClassificationProperties => [.. ProductClassificationSystem, .. ProductClassId, .. ClassificationSystemVersion];
    }
}
