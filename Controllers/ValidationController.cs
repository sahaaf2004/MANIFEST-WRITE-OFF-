using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using MyApp.Services;

namespace MyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ValidationController : ControllerBase
{
    private readonly IMockValidationService _validationService;

    public ValidationController(IMockValidationService validationService)
    {
        _validationService = validationService;
    }

    [HttpPost]
    public async Task<ActionResult<ValidationResponse>> Validate([FromBody] ValidationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RNumber) || string.IsNullOrWhiteSpace(request.Port))
            return BadRequest(new { error = "RNumber and Port are required" });

        var result = await _validationService.ValidateAsync(request);
        return Ok(result);
    }
}
