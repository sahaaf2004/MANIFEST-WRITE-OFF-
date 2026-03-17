namespace MyApp.Models;

public class PaymentConfirmResponse
{
    public string WriteOffRef { get; set; } = string.Empty;
    public string CertificateUrl { get; set; } = string.Empty;
    public string Status { get; set; } = "finalized";
    public string FinalizedAt { get; set; } = string.Empty;
}
