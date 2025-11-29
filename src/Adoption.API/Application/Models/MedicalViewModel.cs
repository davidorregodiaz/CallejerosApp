namespace Adoption.API.Application.Models;

public record MedicalViewModel(
    string Vaccine,
    bool IsStirilized,
    bool IsDewormed,
    string HealthState);
