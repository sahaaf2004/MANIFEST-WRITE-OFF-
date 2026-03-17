namespace MyApp.Models;

public class AmendmentSubmitResponse
{
    public string AmendmentRef { get; set; } = string.Empty;
    public string SubmittedAt { get; set; } = string.Empty;
    public int FineAmount { get; set; }
    public string Status { get; set; } = "pending_review";
}
