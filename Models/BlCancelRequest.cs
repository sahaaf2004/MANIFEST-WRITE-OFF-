namespace MyApp.Models;

public class BlCancelRequest
{
    public string RNumber { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string BlNumber { get; set; } = string.Empty;
    public string FfAccount { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public List<string> DocumentUrls { get; set; } = [];
    public bool DeclarationAccepted { get; set; }
    public string SignatoryDetails { get; set; } = string.Empty;
}
