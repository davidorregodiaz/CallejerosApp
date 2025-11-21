namespace Adoption.API.Application.Services.Email;

public interface IEmailService
{
    Task SendEmailAsync(EmailRequest request);
}
