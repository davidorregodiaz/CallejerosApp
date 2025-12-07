using MailKit.Net.Smtp;
using Adoption.API.Utils.Options;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Adoption.API.Application.Services.Email;

public class EmailService(IOptions<EmailOptions> options, IEmailTemplateService emailTemplateService, ILogger<EmailService> logger) : IEmailService
{
    public async Task SendEmailAsync(EmailRequest emailRequest)
    {
        logger.LogInformation("Processing email request @{EmailRequest}", emailRequest);
        
        var emailTemplate = await emailTemplateService.BuildTemplate(emailRequest.TemplateType, emailRequest.Data);
        var email = BuildMailMessage(emailRequest.To, emailRequest.Subject, emailTemplate);
        var  smtpClient = await BuildSmtpClient();
        await SendEmailAsync(smtpClient, email);
        await DisconnectSmtpClient(smtpClient);
        
        logger.LogInformation("Email sent successfully");
    }
    private async Task DisconnectSmtpClient(SmtpClient smtpClient) => await smtpClient.DisconnectAsync(true);
    private async Task SendEmailAsync(SmtpClient smtpClient,MimeMessage email) => await smtpClient.SendAsync(email);
    private MimeMessage BuildMailMessage(string to, string subject, string message)
    {
        var email = new MimeMessage();
        
        email.From.Add(new MailboxAddress("CallejerosApp Team", options.Value.Username));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        var builder = new BodyBuilder
        {
            HtmlBody = message
        };

        email.Body = builder.ToMessageBody();
        
        return email;
    }
    private async Task<SmtpClient> BuildSmtpClient()
    {
        var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync(options.Value.SmtpHost, options.Value.SmtpPort, options.Value.UseSsl);
        await smtpClient.AuthenticateAsync(options.Value.Username, options.Value.Password);
        return smtpClient;
    }
}
