using lib.Services;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly BroennoeysundApiService _broennoeysundApiService;

    public CompanyController(BroennoeysundApiService broennoeysundApiService)
    {
        _broennoeysundApiService = broennoeysundApiService;
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetCompaniesByName(string name)
    {
        var items = await _broennoeysundApiService.LookupCompanies(name);
        return new OkObjectResult(items);
    }
}