using Adoption.API.Utils.Options;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Adoption.API.Application.Services;

public class MinioService : IMinioService
{
    private readonly IOptions<MinioOptions> _options;
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinioService> _logger;

    public MinioService(IOptions<MinioOptions> options, ILogger<MinioService> logger)
    {
        _options = options;
        _logger = logger;
        _minioClient = new MinioClient()
            .WithEndpoint(_options.Value.Endpoint)
            .WithCredentials(_options.Value.AccessKey, _options.Value.SecretKey)
            .WithSSL(_options.Value.WithSsl)
            .Build();
    }

    public async Task<string> UploadBlob(IFormFile file, string? previousUrl, CancellationToken ct)
    {
        try
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Archivo inválido");

            var ext = Path.GetExtension(file.FileName).ToLower();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".webp" };
            if (!allowedExtensions.Contains(ext))
                throw new InvalidOperationException(
                    $"Extensión de archivo no permitida: {ext}. Extensiones permitidas: {string.Join(", ", allowedExtensions)}");
            
            var connectionValid = await ValidateConnection(ct);
            if (!connectionValid)
            {
                throw new Exception("No se puede conectar a MinIO. Verifica la configuración.");
            }

            await CreateBucketIfNotExistsAsync(ct);

            var fileId = Guid.NewGuid().ToString();
            var objectPath = $"images/{fileId}{ext}";
            
            if (!string.IsNullOrEmpty(previousUrl))
            {
                try
                {
                    await DeleteBlob(previousUrl, ct);
                    _logger.LogInformation($"Archivo anterior eliminado: {previousUrl}");
                }
                catch (Exception deleteEx)
                {
                    _logger.LogWarning("Error eliminando archivo anterior (continuando): {DeleteExMessage}", deleteEx.Message);
                }
            }

            await using var fileStream = file.OpenReadStream();
            var uploadArgs = new PutObjectArgs()
                .WithBucket(_options.Value.BucketName)
                .WithObject(objectPath)
                .WithStreamData(fileStream)
                .WithObjectSize(file.Length)
                .WithContentType(file.ContentType ?? "application/octet-stream");

            await _minioClient.PutObjectAsync(uploadArgs, ct);
            _logger.LogInformation("Subida exitosa: {ObjectPath}", objectPath);

            var exists = await ValidateBlobExistance(objectPath, ct);

            if (!exists)
            {
                throw new Exception("El archivo no se guardó correctamente en MinIO");
            }

            return objectPath;
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("XML") || ex.Message.Contains("document"))
            {
                throw new Exception(
                    $"Error de conectividad con MinIO. Verifica la configuración del endpoint y credenciales. Error original: {ex.Message}");
            }

            throw;
        }
    }

    public async Task<bool> BucketExistsAsync(CancellationToken ct)
    {
        try
        {
            var buckets = await _minioClient.ListBucketsAsync(ct);
            return buckets.Buckets.Any(b => b.Name == _options.Value.BucketName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error verificando el bucket: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ValidateBlobExistance(string url, CancellationToken ct)
    {
        try
        {
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(_options.Value.BucketName)
                .WithObject(url);

            await _minioClient.StatObjectAsync(statObjectArgs, ct);
            return true;
        }
        catch
        {
            return false;
        }
    }
    private async Task CreateBucketIfNotExistsAsync(CancellationToken ct)
    {
        try
        {
            if (!await BucketExistsAsync(ct))
            {
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(_options.Value.BucketName);
                await _minioClient.MakeBucketAsync(makeBucketArgs, ct);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creando el bucket: {ex.Message}");
            throw;
        }
    }

    public async Task<string> PresignedGetUrl(string objPath, CancellationToken ct)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(_options.Value.BucketName)
            .WithObject(objPath)
            .WithExpiry(60 * 60 * 24 * 7);
        return await _minioClient.PresignedGetObjectAsync(args);
    }

    public async Task DeleteBlob(string url, CancellationToken ct)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(_options.Value.BucketName)
            .WithObject(url);

        await _minioClient.RemoveObjectAsync(args, ct);
    }
    
    public async Task<bool> ValidateConnection(CancellationToken ct = default)
    {
        try
        {
            var buckets = await _minioClient.ListBucketsAsync(ct);
            var targetBucketExists = buckets.Buckets.Any(b => b.Name == _options.Value.BucketName);
            
            if (targetBucketExists)
            {
                _logger.LogInformation("Bucket configurado '{BucketName}' encontrado", _options.Value.BucketName);
            }
            else
            {
                _logger.LogWarning("Bucket configurado '{BucketName}' no encontrado", _options.Value.BucketName);
                _logger.LogInformation(
                    "Buckets disponibles: {Buckets}", string.Join(", ", buckets.Buckets.Select(b => b.Name)));
            }

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
