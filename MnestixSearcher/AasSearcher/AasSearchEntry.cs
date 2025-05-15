using MongoDB.Bson.Serialization.Attributes;

namespace MnestixSearcher.AasSearcher;

public class AasSearchEntry
{
    [BsonId]
    public string? Id { get; set; }
    
    public string? ProductRoot { get; set; }
    
    public string? ProductFamily { get; set; }
    
    public string? ProductDesignation { get; set; }
    
   // public Dictionary<string, object>? TechnicalProperties { get; set; }
    
    public Dictionary<string, object>? ProductClassifications { get; set; }
}