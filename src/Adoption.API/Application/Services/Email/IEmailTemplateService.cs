namespace Adoption.API.Application.Services.Email;

public interface IEmailTemplateService
{
    Task<string> BuildTemplate(EmailTemplateType type, Dictionary<string, string> data);
}

