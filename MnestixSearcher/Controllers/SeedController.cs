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
    private readonly ISeedService _seedService;

    public SeedController(ILogger<SeedController> logger, ISeedService searchService)
    {
        _logger = logger;
        _seedService = searchService;
    }
    
    [HttpPost]
    public async Task<IActionResult> Post()
    {
        await _seedService.SeedDatabase();
        return NoContent();
    }
    
    [HttpGet]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<List<AasSearchEntry>> Get() =>
        await _seedService.GetAsync();
}
