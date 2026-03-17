namespace MyApp.Models;

public class PaymentConfirmRequest
{
    public string PaymentRef { get; set; } = string.Empty;
    public string TransactionRef { get; set; } = string.Empty;
}
