
using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Services.Email;

public class EmailTemplateService : IEmailTemplateService
{
    public async Task<string> BuildTemplate(EmailTemplateType type, Dictionary<string, string> data)
    {
        return type switch
        {
            EmailTemplateType.Welcome => await BuildWelcomeTemplate(data),
            EmailTemplateType.AdoptionRequestCreated => await BuildCreatedAdoptionRequestTemplate(data),
            EmailTemplateType.AdoptionRequestStatusChange =>  await BuildAdoptionRequestStatusChangeTemplate(data),
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Template type not supported.")
        };
    }

    private async Task<string> BuildWelcomeTemplate(Dictionary<string, string> data)
    {
        var userName = data.TryGetValue("AppUser", out var value) ? value : "Usuario";

        var template = await File.ReadAllTextAsync("./Application/Services/Email/EmailTemplates/welcome.html");

        template = template
            .Replace("{{username}}", userName);
        
        return template;
    }

    private async Task<string> BuildAdoptionRequestStatusChangeTemplate(Dictionary<string, string> data)
    {
        string template = string.Empty;
        
        var animalName =  data.TryGetValue("AnimalName", out var name) ? name : string.Empty;
        var animalBreed = data.TryGetValue("AnimalBreed", out var breed) ? breed : string.Empty;
        var ownerName = data.TryGetValue("OwnerName", out var owner) ? owner :  string.Empty;
        var requestStatus = data.TryGetValue("Status", out var status) ? status :  string.Empty;
        
        if(string.Equals(requestStatus, nameof(AdoptionStatus.Approved), StringComparison.OrdinalIgnoreCase))
            template = await File.ReadAllTextAsync("./Application/Services/Email/EmailTemplates/adoption-status-approved.html");
            
        if(string.Equals(requestStatus, nameof(AdoptionStatus.Rejected), StringComparison.OrdinalIgnoreCase))
            template = await File.ReadAllTextAsync("./Application/Services/Email/EmailTemplates/adoption-status-rejected.html");

        template = template
            .Replace("{{animalName}}", animalName)
            .Replace("{{animalBreed}}", animalBreed)
            .Replace("{{ownerName}}", ownerName);
        
        return template;
    }

    private async Task<string> BuildCreatedAdoptionRequestTemplate(Dictionary<string, string> data)
    {
        var animalName =  data.TryGetValue("AnimalName", out var name) ? name : string.Empty;
        var animalBreed = data.TryGetValue("AnimalBreed", out var breed) ? breed : string.Empty;
        var requesterName = data.TryGetValue("RequesterName", out var requester) ? requester :  string.Empty;
        var requestDate = data.TryGetValue("RequestDate", out var request) ? request :  string.Empty;
        
        string template = await File.ReadAllTextAsync("./Application/Services/Email/EmailTemplates/adoption-created.html");

        template = template
            .Replace("{{animalName}}", animalName)
            .Replace("{{animalBreed}}", animalBreed)
            .Replace("{{requesterName}}", requesterName)
            .Replace("{{date}}", requestDate);
        
        return template;
    }
    
}

