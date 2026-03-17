using System.Collections.Concurrent;
using MyApp.Models;

namespace MyApp.Services;

public class MockAmendmentService : IMockAmendmentService
{
    // Stores amendment state: ref → (submittedAt, fineAmount, forceReject)
    private static readonly ConcurrentDictionary<string, AmendmentRecord> Store = new();

    public async Task<AmendmentSubmitResponse> SubmitAsync(AmendmentSubmitRequest request)
    {
        await Task.Delay(800);

        var year = DateTime.UtcNow.Year;
        var suffix = Random.Shared.Next(100000, 999999);
        var amendmentRef = $"AMD-{year}-{suffix}";
        var now = DateTime.UtcNow;

        // Fine: late manifest = 3000, Tier II mismatch = 1000, Tier I = 500
        var fine = CalculateFine(request.ServiceTypes, request.AmendmentTypes);

        // If any service type contains "late" → late manifest fine applies
        var record = new AmendmentRecord(now, fine, ForceReject: false);
        Store[amendmentRef] = record;

        return new AmendmentSubmitResponse
        {
            AmendmentRef = amendmentRef,
            SubmittedAt = now.ToString("O"),
            FineAmount = fine,
            Status = "pending_review"
        };
    }

    public async Task<AmendmentStatusResponse?> GetStatusAsync(string amendmentRef)
    {
        await Task.Delay(300);

        if (!Store.TryGetValue(amendmentRef, out var record))
            return null;

        var elapsed = DateTime.UtcNow - record.SubmittedAt;

        // Refs ending in "-REJ" (or manually forced) return rejected at step 1
        if (record.ForceReject || amendmentRef.EndsWith("-REJ", StringComparison.OrdinalIgnoreCase))
        {
            return new AmendmentStatusResponse
            {
                Status = "rejected",
                ReviewStep = 1,
                RejectionReason = "The amendment details submitted do not match the original manifest records. Please review and resubmit with accurate information."
            };
        }

        // Time-based progression through 4 review steps (~20s each in demo)
        // In production this would be driven by officer actions, not time
        return elapsed.TotalSeconds switch
        {
            < 20 => Step("pending_review", 1),
            < 40 => Step("pending_review", 2),
            < 60 => Step("pending_review", 3),
            < 80 => Step("pending_review", 4),
            _    => new AmendmentStatusResponse { Status = "approved", ReviewStep = 4 }
        };
    }

    private static AmendmentStatusResponse Step(string status, int step) =>
        new() { Status = status, ReviewStep = step };

    private static int CalculateFine(List<string> serviceTypes, List<string> amendmentTypes)
    {
        // Late manifest always carries the flat MVR 3,000 fine
        if (serviceTypes.Any(s => s.Contains("late", StringComparison.OrdinalIgnoreCase)))
            return 3000;

        // Amendment types that map to Tier II (MVR 1,000): consignee, carrier
        var tierTwoFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "Consignee", "Carrier" };

        if (amendmentTypes.Any(f => tierTwoFields.Contains(f)))
            return 1000;

        // Everything else is Tier I (MVR 500)
        return amendmentTypes.Count > 0 ? 500 : 0;
    }

    private record AmendmentRecord(DateTime SubmittedAt, int FineAmount, bool ForceReject);
}
