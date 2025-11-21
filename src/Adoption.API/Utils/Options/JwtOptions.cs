
namespace Adoption.API.Utils.Options;

public class JwtOptions
{
    public string Url { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string ExpireMinutes { get; set; } = null!;
    public string SigningKey { get; set; } = null!;
}
