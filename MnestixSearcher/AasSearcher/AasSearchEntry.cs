using MongoDB.Bson.Serialization.Attributes;

namespace MnestixSearcher.Controllers;

public class AasSearchEntry
{
    [BsonId]
    public string? Id { get; set; }
    
    public string? ProductRoot { get; set; }
    
    public string? ProductFamily { get; set; }
    
    public string? ProductDesignation { get; set; }
}