using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using MyApp.Services;

namespace MyApp.Controllers;

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly IMockPaymentService _paymentService;

    public PaymentController(IMockPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("initiate")]
    public async Task<ActionResult<PaymentInitiateResponse>> Initiate([FromBody] PaymentInitiateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AmendmentRef))
            return BadRequest(new { error = "AmendmentRef is required" });

        var result = await _paymentService.InitiateAsync(request);
        return Ok(result);
    }

    [HttpPost("confirm")]
    public async Task<ActionResult<PaymentConfirmResponse>> Confirm([FromBody] PaymentConfirmRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PaymentRef))
            return BadRequest(new { error = "PaymentRef is required" });

        var result = await _paymentService.ConfirmAsync(request);
        if (result is null)
            return NotFound(new { error = $"Payment '{request.PaymentRef}' not found" });

        return Ok(result);
    }
}
