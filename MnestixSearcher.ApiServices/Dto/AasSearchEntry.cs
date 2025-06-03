using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.InteropServices;

namespace MnestixSearcher.AasSearcher;

public class AasSearchEntry
{
    [BsonId]
    public string? Id { get; set; }
    public DateTime? CreatedTime { get; set; }
    public string? ThumbnailUrl { get; set; }
    public PropertyData? ManufacturerName { get; set; }
    public PropertyData? ProductRoot { get; set; }
    public PropertyData? ProductFamily { get; set; }
    public PropertyData? ProductDesignation { get; set; }

    // public Dictionary<string, object>? TechnicalProperties { get; set; }

    public List<ProductClassificationValues> ProductClassifications { get; set; } = [];
    public Boolean SaveData { get; set; } = false;
}

public class PropertyData
{
    public string SemanticId { get; set; } = string.Empty;
    public string IdShortPath { get; set; } = string.Empty;
    public string? Value { get; set; }
    public List<MLValue>? MLValues { get; set; } = [];
}

public class MLValue
{
    public string Language { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public class ProductClassificationValues
{
    public string System { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
}