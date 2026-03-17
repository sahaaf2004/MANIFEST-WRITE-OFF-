namespace MyApp.Models;

public class ValidationResponse
{
    public string Status { get; set; } = string.Empty;
    public ManifestDataDto? ManifestData { get; set; }
    public List<FieldResultDto> FieldResults { get; set; } = [];
    public List<FieldResultDto> FailedFields { get; set; } = [];
    public int CalculatedFine { get; set; }
    public string? WriteOffRef { get; set; }
    public string? ErrorMessage { get; set; }
}
