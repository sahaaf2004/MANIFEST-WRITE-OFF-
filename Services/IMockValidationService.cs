using MyApp.Models;

namespace MyApp.Services;

public interface IMockValidationService
{
    Task<ValidationResponse> ValidateAsync(ValidationRequest request);
}
