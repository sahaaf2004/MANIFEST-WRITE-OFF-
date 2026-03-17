namespace MyApp.Models;

public class FieldResultDto
{
    public string Field { get; set; } = string.Empty;
    public string ManifestValue { get; set; } = string.Empty;
    public string EvaluatorValue { get; set; } = string.Empty;
    public bool Match { get; set; }
    public int FineTier { get; set; }
}
