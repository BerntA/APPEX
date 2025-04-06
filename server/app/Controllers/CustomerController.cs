using lib.Models.DTOs;
using lib.Services;
using Microsoft.AspNetCore.Mvc;

namespace server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly CustomerService _customerService;

    public CustomerController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        var items = await _customerService.GetCustomers();
        return new OkObjectResult(items);
    }

    [HttpPost]
    public async Task<IActionResult> AddCustomer([FromBody] CustomerDto customer)
    {
        var result = await _customerService.AddCustomer(customer);
        return (result is null) ? new ConflictResult() : new OkObjectResult(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCustomer([FromBody] CustomerDto customer)
    {
        var result = await _customerService.UpdateCustomer(customer);
        return (result is null) ? new NotFoundResult() : new OkObjectResult(result);
    }

    [HttpDelete("{organizationNumber}")]
    public async Task<IActionResult> RemoveCustomer(string organizationNumber)
    {
        var result = await _customerService.RemoveCustomer(organizationNumber);
        return (result is null) ? new NotFoundResult() : new OkObjectResult(result);
    }
}