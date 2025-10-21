namespace Client.Models;

public record TokenModel(string Token, int ExpirationMinutes, string RefreshToken);
