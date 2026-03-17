namespace MyApp.Models;

public class AmendmentStatusResponse
{
    // "pending_review" | "approved" | "rejected"
    public string Status { get; set; } = string.Empty;
    // 1–4 for the 4-step progress tracker shown on S8
    public int ReviewStep { get; set; }
    public string? RejectionReason { get; set; }
}
