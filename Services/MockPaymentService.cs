using System.Collections.Concurrent;
using System.Text;
using MyApp.Models;

namespace MyApp.Services;

public class MockPaymentService : IMockPaymentService
{
    // payment_ref → (amendment_ref, amount, initiated_at)
    private static readonly ConcurrentDictionary<string, PaymentRecord> PaymentStore = new();
    // write_off_ref → payment_record (set on confirm)
    private static readonly ConcurrentDictionary<string, PaymentRecord> WriteOffStore = new();

    public async Task<PaymentInitiateResponse> InitiateAsync(PaymentInitiateRequest request)
    {
        await Task.Delay(500);

        var year = DateTime.UtcNow.Year;
        var suffix = Random.Shared.Next(100000, 999999);
        var paymentRef = $"PAY-{year}-{suffix}";

        // Look up fine amount from amendment ref (just encode it in mock)
        // In prod this would hit the DB; for mock we return a fixed amount
        var amount = 1000; // Will be updated when frontend passes amount

        var record = new PaymentRecord(request.AmendmentRef, amount, DateTime.UtcNow);
        PaymentStore[paymentRef] = record;

        return new PaymentInitiateResponse
        {
            PaymentRef = paymentRef,
            RedirectUrl = null, // deferred_account goes straight through
            Amount = amount,
            Currency = "MVR",
            Status = "initiated"
        };
    }

    public async Task<PaymentConfirmResponse?> ConfirmAsync(PaymentConfirmRequest request)
    {
        await Task.Delay(800);

        if (!PaymentStore.TryGetValue(request.PaymentRef, out var record))
            return null;

        var year = DateTime.UtcNow.Year;
        var suffix = Random.Shared.Next(100000, 999999);
        var writeOffRef = $"WO-{year}-{suffix}";
        var now = DateTime.UtcNow;

        WriteOffStore[writeOffRef] = record;

        return new PaymentConfirmResponse
        {
            WriteOffRef = writeOffRef,
            CertificateUrl = $"/api/certificate/{writeOffRef}",
            Status = "finalized",
            FinalizedAt = now.ToString("O")
        };
    }

    public byte[] GenerateCertificatePdf(string writeOffRef)
    {
        // Minimal valid PDF — no external library required
        var date = DateTime.UtcNow.ToString("dd MMMM yyyy");
        var content = new StringBuilder();

        content.AppendLine("%PDF-1.4");
        content.AppendLine("1 0 obj");
        content.AppendLine("<< /Type /Catalog /Pages 2 0 R >>");
        content.AppendLine("endobj");
        content.AppendLine("2 0 obj");
        content.AppendLine("<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
        content.AppendLine("endobj");
        content.AppendLine("3 0 obj");
        content.AppendLine("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842]");
        content.AppendLine("   /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >>");
        content.AppendLine("endobj");

        var stream = BuildCertificateStream(writeOffRef, date);
        content.AppendLine("4 0 obj");
        content.AppendLine($"<< /Length {stream.Length} >>");
        content.AppendLine("stream");
        content.Append(stream);
        content.AppendLine("endstream");
        content.AppendLine("endobj");
        content.AppendLine("5 0 obj");
        content.AppendLine("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");
        content.AppendLine("endobj");
        content.AppendLine("xref");
        content.AppendLine("0 6");
        content.AppendLine("0000000000 65535 f");
        content.AppendLine("0000000009 00000 n");
        content.AppendLine("0000000062 00000 n");
        content.AppendLine("0000000119 00000 n");
        content.AppendLine("0000000273 00000 n");
        content.AppendLine("0000000392 00000 n");
        content.AppendLine("trailer");
        content.AppendLine("<< /Size 6 /Root 1 0 R >>");
        content.AppendLine("startxref");
        content.AppendLine("492");
        content.AppendLine("%%EOF");

        return Encoding.ASCII.GetBytes(content.ToString());
    }

    private static string BuildCertificateStream(string writeOffRef, string date) =>
        $"BT\n" +
        $"/F1 18 Tf\n" +
        $"100 760 Td\n" +
        $"(MALDIVES CUSTOMS SERVICE) Tj\n" +
        $"/F1 14 Tf\n" +
        $"0 -30 Td\n" +
        $"(MANIFEST WRITE-OFF CERTIFICATE) Tj\n" +
        $"/F1 11 Tf\n" +
        $"0 -50 Td\n" +
        $"(Reference Number: {writeOffRef}) Tj\n" +
        $"0 -20 Td\n" +
        $"(Date Issued: {date}) Tj\n" +
        $"0 -40 Td\n" +
        $"(This certifies that the manifest write-off has been approved and) Tj\n" +
        $"0 -20 Td\n" +
        $"(all outstanding discrepancies have been resolved.) Tj\n" +
        $"0 -40 Td\n" +
        $"(This document is computer-generated and does not require a signature.) Tj\n" +
        $"ET\n";

    private record PaymentRecord(string AmendmentRef, int Amount, DateTime InitiatedAt);
}
