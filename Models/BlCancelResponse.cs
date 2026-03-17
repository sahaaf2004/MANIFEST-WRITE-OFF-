namespace MyApp.Models;

public class BlCancelResponse
{
    public string CancelRef { get; set; } = string.Empty;
    public string SubmittedAt { get; set; } = string.Empty;
    public string Status { get; set; } = "submitted";
    public string Message { get; set; } = string.Empty;
}
