using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace MnestixSearcher.AasSearcher;

[ApiController]
[Route("api/[controller]")]
public class AasSearcherController : ControllerBase
{
    private readonly ILogger<AasSearcherController> _logger;
    private readonly AasSearcherService _searchService;

    public AasSearcherController(ILogger<AasSearcherController> logger, AasSearcherService searchService)
    {
        _logger = logger;
        _searchService = searchService;
    }
    
    [HttpPost]
    public async Task<IActionResult> Post()
    {
        await _searchService.FillDatabase();
        return NoContent();
    }
    
    [HttpGet]
    public async Task<List<AasSearchEntry>> Get() =>
        await _searchService.GetAsync();
    
    [HttpGet("search")]
    public async Task<List<AasSearchEntry>> GetByCriteria([FromQuery] string? productRoot = null, [FromQuery] string? productFamily = null,
        [FromQuery] string? productDesignation = null, [FromQuery] Dictionary<string, string>? classification = null)
    {
        var filters = new List<FilterDefinition<AasSearchEntry>>();

        if (!string.IsNullOrEmpty(productRoot))
            filters.Add(Builders<AasSearchEntry>.Filter.Eq(entry => entry.ProductRoot, productRoot));

        if (!string.IsNullOrEmpty(productFamily))
            filters.Add(Builders<AasSearchEntry>.Filter.Eq(entry => entry.ProductFamily, productFamily));

        if (!string.IsNullOrEmpty(productDesignation))
            filters.Add(Builders<AasSearchEntry>.Filter.Eq(entry => entry.ProductDesignation, productDesignation));

        if (classification != null)
        {
            foreach (var property in classification)
            {
                filters.Add(Builders<AasSearchEntry>.Filter.Eq($"ProductClassifications.{property.Key}", property.Value));
            }
        }

        var combinedFilter = filters.Count > 0
            ? Builders<AasSearchEntry>.Filter.And(filters)
            : Builders<AasSearchEntry>.Filter.Empty;

        return await _searchService.GetByCriteriaAsync(combinedFilter);
    }

}
