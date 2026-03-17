using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using MyApp.Services;

namespace MyApp.Controllers;

[ApiController]
[Route("api/amendment")]
public class AmendmentController : ControllerBase
{
    private readonly IMockAmendmentService _amendmentService;

    public AmendmentController(IMockAmendmentService amendmentService)
    {
        _amendmentService = amendmentService;
    }

    [HttpPost("submit")]
    public async Task<ActionResult<AmendmentSubmitResponse>> Submit([FromBody] AmendmentSubmitRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RNumber) || string.IsNullOrWhiteSpace(request.BlNumber))
            return BadRequest(new { error = "RNumber and BlNumber are required" });

        if (!request.DeclarationAccepted)
            return BadRequest(new { error = "Declaration must be accepted before submission" });

        var result = await _amendmentService.SubmitAsync(request);
        return Ok(result);
    }

    [HttpGet("status/{amendmentRef}")]
    public async Task<ActionResult<AmendmentStatusResponse>> GetStatus(string amendmentRef)
    {
        if (string.IsNullOrWhiteSpace(amendmentRef))
            return BadRequest(new { error = "amendmentRef is required" });

        var result = await _amendmentService.GetStatusAsync(amendmentRef);
        if (result is null)
            return NotFound(new { error = $"Amendment '{amendmentRef}' not found" });

        return Ok(result);
    }
}
