namespace MyApp.Models;

public class PaymentInitiateRequest
{
    public string AmendmentRef { get; set; } = string.Empty;
    // "deferred_account" | "bank_transfer" | "card"
    public string PaymentMethod { get; set; } = "deferred_account";
}
