using Microsoft.AspNetCore.Mvc;
using MyApp.Models;

namespace MyApp.Controllers;

[ApiController]
[Route("api/bl-cancel")]
public class BlCancelController : ControllerBase
{
    [HttpPost("submit")]
    public async Task<ActionResult<BlCancelResponse>> Submit([FromBody] BlCancelRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.BlNumber) || string.IsNullOrWhiteSpace(request.RNumber))
            return BadRequest(new { error = "BlNumber and RNumber are required" });

        if (!request.DeclarationAccepted)
            return BadRequest(new { error = "Declaration must be accepted before submission" });

        // Simulate processing delay
        await Task.Delay(600);

        var year = DateTime.UtcNow.Year;
        var suffix = Random.Shared.Next(100000, 999999);
        var cancelRef = $"BLC-{year}-{suffix}";

        return Ok(new BlCancelResponse
        {
            CancelRef = cancelRef,
            SubmittedAt = DateTime.UtcNow.ToString("O"),
            Status = "submitted",
            Message = $"BL cancellation request for '{request.BlNumber}' has been submitted and is pending review by a Customs officer."
        });
    }
}
