using MyApp.Models;

namespace MyApp.Services;

public interface IMockPaymentService
{
    Task<PaymentInitiateResponse> InitiateAsync(PaymentInitiateRequest request);
    Task<PaymentConfirmResponse?> ConfirmAsync(PaymentConfirmRequest request);
    byte[] GenerateCertificatePdf(string writeOffRef);
}
