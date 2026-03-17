namespace MyApp.Models;

public class AmendmentSubmitRequest
{
    public string RNumber { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string BlNumber { get; set; } = string.Empty;
    public List<string> ServiceTypes { get; set; } = [];
    public List<string> AmendmentTypes { get; set; } = [];
    public string ContentBefore { get; set; } = string.Empty;
    public string ContentAfter { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string FfAccount { get; set; } = string.Empty;
    public List<string> DocumentUrls { get; set; } = [];
    public bool DeclarationAccepted { get; set; }
    public string SignatoryDetails { get; set; } = string.Empty;
}
