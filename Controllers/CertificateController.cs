using Microsoft.AspNetCore.Mvc;
using MyApp.Services;

namespace MyApp.Controllers;

[ApiController]
[Route("api/certificate")]
public class CertificateController : ControllerBase
{
    private readonly IMockPaymentService _paymentService;

    public CertificateController(IMockPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet("{writeOffRef}")]
    public IActionResult GetCertificate(string writeOffRef)
    {
        if (string.IsNullOrWhiteSpace(writeOffRef))
            return BadRequest(new { error = "writeOffRef is required" });

        var pdfBytes = _paymentService.GenerateCertificatePdf(writeOffRef);
        var fileName = $"WriteOff-{writeOffRef}.pdf";

        return File(pdfBytes, "application/pdf", fileName);
    }
}
