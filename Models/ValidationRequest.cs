namespace MyApp.Models;

public class ValidationRequest
{
    public string RNumber { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public int Year { get; set; }
    public string DeferredPaymentAccount { get; set; } = string.Empty;
}
