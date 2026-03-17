using MyApp.Models;

namespace MyApp.Services;

public interface IMockAmendmentService
{
    Task<AmendmentSubmitResponse> SubmitAsync(AmendmentSubmitRequest request);
    Task<AmendmentStatusResponse?> GetStatusAsync(string amendmentRef);
}
