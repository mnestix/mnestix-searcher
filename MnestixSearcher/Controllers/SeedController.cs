using Microsoft.AspNetCore.Mvc;
using MnestixSearcher.AasSearcher;
using MnestixSearcher.ApiServices.Contracts;
using MnestixSearcher.ApiServices.Dto;
using MnestixSearcher.Authorization;
using MongoDB.Driver;

namespace MnestixSearcher.Controllers;

[ApiKey]
[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly ILogger<SeedController> _logger;
    private readonly IAasSearcherService _searchService;

    public SeedController(ILogger<SeedController> logger, IAasSearcherService searchService)
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
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<List<AasSearchEntry>> Get() =>
        await _searchService.GetAsync();
    
    [HttpGet("search")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<List<AasSearchEntry>> GetByCriteria([FromQuery] SearchCriteria? criteria = null)
    {
        var filters = new List<FilterDefinition<AasSearchEntry>>();

        if (!string.IsNullOrEmpty(criteria?.ProductRoot))
            filters.Add(Builders<AasSearchEntry>.Filter.Eq("ProductRoot.MLValues.Text", criteria.ProductRoot));

        if (!string.IsNullOrEmpty(criteria?.ProductFamily))
            filters.Add(Builders<AasSearchEntry>.Filter.Eq("ProductFamily.MLValues.Text", criteria.ProductFamily));

        if (!string.IsNullOrEmpty(criteria?.ProductDesignation))
            filters.Add(Builders<AasSearchEntry>.Filter.Eq("ProductDesignation.MLValues.Text", criteria.ProductDesignation));

        if (criteria?.Classification != null)
        {
            foreach (var property in criteria.Classification)
            {
                filters.Add(Builders<AasSearchEntry>.Filter.Eq($"ProductClassifications.{property.Key}.ProductId", property.Value));
            }
        }

        var combinedFilter = filters.Count != 0
            ? Builders<AasSearchEntry>.Filter.And(filters)
            : Builders<AasSearchEntry>.Filter.Empty;

        return await _searchService.GetByCriteriaAsync(combinedFilter);
    }

}
