namespace Adoption.API.Utils.Options;

public class EmailOptions
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 465;
    // public string ImapHost { get; set; } = string.Empty;
    // public int ImapPort { get; set; } = 993;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    // public string CompanyEmail { get; set; } = string.Empty;
    // public List<string> SupportEmails { get; set; } = new List<string>();
    // public string UnsubscribeEmail { get; set; } = string.Empty;
    public bool UseSsl { get; set; }
    public bool EnableAntiSpamHeaders { get; set; }
    public int TimeoutSeconds { get; set; }
}
