using Microsoft.AspNetCore.Mvc;
using MnestixSearcher.Controllers;

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
        //Fill Database
        await _searchService.FillDatabase();
        return NoContent();
    }
    
    [HttpGet]
    public async Task<List<AasSearchEntry>> Get() =>
        await _searchService.GetAsync();

}
