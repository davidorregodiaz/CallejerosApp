namespace Adoption.API.Application.Services.Minio;

public interface IMinioService
{
    public Task<string> UploadBlob(IFormFile file, string? previousUrl, CancellationToken ct);

    public Task DeleteBlob(string url, CancellationToken ct);

    public Task<bool> ValidateBlobExistance(string url, CancellationToken ct);

    public Task<string> PresignedGetUrl(string objPath, CancellationToken ct);

    public Task<bool> ValidateConnection(CancellationToken ct = default);
}
