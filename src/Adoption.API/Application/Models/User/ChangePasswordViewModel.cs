namespace Adoption.API.Application.Models.User;

public record ChangePasswordViewModel(
    string CurrentPassword,
    string NewPassword);
