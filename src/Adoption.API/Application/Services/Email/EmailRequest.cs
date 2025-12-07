namespace Adoption.API.Application.Services.Email;

public record EmailRequest(
    string To,
    string Subject,
    Dictionary<string, string> Data,
    EmailTemplateType TemplateType);
