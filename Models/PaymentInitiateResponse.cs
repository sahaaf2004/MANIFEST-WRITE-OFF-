namespace MyApp.Models;

public class PaymentInitiateResponse
{
    public string PaymentRef { get; set; } = string.Empty;
    public string? RedirectUrl { get; set; }
    public int Amount { get; set; }
    public string Currency { get; set; } = "MVR";
    public string Status { get; set; } = "initiated";
}
