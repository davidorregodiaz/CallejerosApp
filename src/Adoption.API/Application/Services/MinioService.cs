namespace Adoption.API.Application.Services;

public class MinioService : IMinioService
{
    public Task<string> UploadFileAsync(IFormFile image)
    {
        return  Task.FromResult("Imagen URL de prueba");
    }
}
