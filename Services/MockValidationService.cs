using MyApp.Models;

namespace MyApp.Services;

public class MockValidationService : IMockValidationService
{
    // Realistic Maldivian manifest data used across all scenarios
    private static readonly ManifestDataDto SampleManifest = new()
    {
        Vessel = "MV SITC KEELUNG",
        Voyage = "2401N",
        DateArrival = "2024-03-15",
        BlNumber = "SITCMLE2401001",
        FfAccount = "102",
        ConsigneeCode = "MLE0042",
        GrossWeight = "12500",
        PackageCode = "CTN",
        PacksNumber = "48",
        Exporter = "GUANGZHOU TRADING CO LTD",
        Consignee = "MALE GENERAL TRADING PVT LTD",
        Carrier = "SITC CONTAINER LINES CO LTD"
    };

    public async Task<ValidationResponse> ValidateAsync(ValidationRequest request)
    {
        // Simulate network latency
        await Task.Delay(1500);

        var prefix = request.RNumber.ToUpperInvariant();

        if (prefix.StartsWith("DOWN"))
            return ErrorResponse("asycuda_down", "ASYCUDA World is currently unavailable. Please try again.");

        if (prefix.StartsWith("NF"))
            return ErrorResponse("r_not_found", $"R-Number '{request.RNumber}' was not found in ASYCUDA for port '{request.Port}'.");

        if (prefix.StartsWith("NA"))
            return ErrorResponse("not_assessed", "This manifest form has not yet been assessed by a Customs officer.");

        if (prefix.StartsWith("LK"))
            return ErrorResponse("form_locked", "This manifest form is currently locked by a Customs officer.");

        if (prefix.StartsWith("IF"))
            return ErrorResponse("insufficient_funds", "Deferred payment account has insufficient funds to cover processing fees.");

        if (prefix.StartsWith("AM"))
            return ErrorResponse("account_mismatch", "The deferred payment account code does not match the consignee account code.");

        if (prefix.StartsWith("BL"))
            return ErrorResponse("bl_not_found", $"Bill of Lading '{SampleManifest.BlNumber}' was not found in the Tradian BL registry.");

        if (prefix.StartsWith("TE"))
            return ErrorResponse("ecustoms_error", "BL found in Tradian but is not registered in E-Customs. Please contact IT support.");

        if (prefix.StartsWith("FM"))
            return FieldMismatchResponse(request);

        return SuccessResponse(request);
    }

    private static ValidationResponse SuccessResponse(ValidationRequest request)
    {
        var year = request.Year > 0 ? request.Year : DateTime.UtcNow.Year;
        var refSuffix = new Random().Next(100000, 999999);
        var writeOffRef = $"WO-{year}-{refSuffix}";

        var allFields = BuildAllMatchingFields();

        return new ValidationResponse
        {
            Status = "write_off_success",
            ManifestData = SampleManifest,
            FieldResults = allFields,
            FailedFields = [],
            CalculatedFine = 0,
            WriteOffRef = writeOffRef
        };
    }

    private static ValidationResponse FieldMismatchResponse(ValidationRequest request)
    {
        var fields = new List<FieldResultDto>
        {
            // Matching fields
            Field("Gross Weight",   "12500",                          "12500",                          match: true,  tier: 1),
            Field("Package Code",   "CTN",                            "CTN",                            match: true,  tier: 1),
            Field("Packs Number",   "48",                             "48",                             match: true,  tier: 1),
            // Tier I mismatch — 500 MVR
            Field("Exporter",       "GUANGZHOU TRADING CO LTD",       "GUANGZHOU TRADING COMPANY LTD",  match: false, tier: 1),
            // Tier II mismatch — 1000 MVR
            Field("Consignee",      "MALE GENERAL TRADING PVT LTD",   "MALE TRADING PVT LTD",           match: false, tier: 2),
            // Matching fields
            Field("Carrier",        "SITC CONTAINER LINES CO LTD",    "SITC CONTAINER LINES CO LTD",    match: true,  tier: 1),
            Field("BL Number",      "SITCMLE2401001",                  "SITCMLE2401001",                 match: true,  tier: 1),
        };

        var failedFields = fields.Where(f => !f.Match).ToList();

        // Fine = highest tier among mismatched fields (Tier II = 1000, Tier I = 500)
        var calculatedFine = failedFields.Any(f => f.FineTier == 2) ? 1000 : 500;

        return new ValidationResponse
        {
            Status = "field_mismatch",
            ManifestData = SampleManifest,
            FieldResults = fields,
            FailedFields = failedFields,
            CalculatedFine = calculatedFine
        };
    }

    private static ValidationResponse ErrorResponse(string status, string message) =>
        new() { Status = status, ErrorMessage = message };

    private static List<FieldResultDto> BuildAllMatchingFields() =>
    [
        Field("Gross Weight",  "12500",                          "12500",                          match: true, tier: 1),
        Field("Package Code",  "CTN",                            "CTN",                            match: true, tier: 1),
        Field("Packs Number",  "48",                             "48",                             match: true, tier: 1),
        Field("Exporter",      "GUANGZHOU TRADING CO LTD",       "GUANGZHOU TRADING CO LTD",       match: true, tier: 1),
        Field("Consignee",     "MALE GENERAL TRADING PVT LTD",   "MALE GENERAL TRADING PVT LTD",   match: true, tier: 1),
        Field("Carrier",       "SITC CONTAINER LINES CO LTD",    "SITC CONTAINER LINES CO LTD",    match: true, tier: 1),
        Field("BL Number",     "SITCMLE2401001",                  "SITCMLE2401001",                 match: true, tier: 1),
    ];

    private static FieldResultDto Field(string name, string manifestVal, string evaluatorVal, bool match, int tier) =>
        new()
        {
            Field = name,
            ManifestValue = manifestVal,
            EvaluatorValue = evaluatorVal,
            Match = match,
            FineTier = tier
        };
}
