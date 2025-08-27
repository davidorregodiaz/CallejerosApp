using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Client.Models;
using Shared;
using Shared.Dtos;

namespace Client.Services;

public class AnimalService
{
    private const int MaxFileSize = 2 * 1024 * 1024; // 2 MB
    private readonly IHttpClientFactory _clientFactory;
    public AnimalService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    
    public async Task<TaskResult<IEnumerable<ResponseAnimalDto>>> GetAnimalsAsync()
    {
        var client = _clientFactory.CreateClient("Api");
        try
        {
            var response = await client.GetAsync("api/animals");
            response.EnsureSuccessStatusCode();
            var animals = await response.Content.ReadFromJsonAsync<List<ResponseAnimalDto>>();
            return TaskResult<IEnumerable<ResponseAnimalDto>>.FromData(animals!);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
            return TaskResult<IEnumerable<ResponseAnimalDto>>.FromFailure("Not animals available.");
        }
    }

    public async Task<TaskResult<ResponseAnimalDto>> GetAnimalByIdAsync(Guid id)
    {
        var client = _clientFactory.CreateClient("Api");
        try
        {
            var response = await client.GetAsync($"api/animals/{id}");
            response.EnsureSuccessStatusCode();
            var animal = await response.Content.ReadFromJsonAsync<ResponseAnimalDto>();
            return TaskResult<ResponseAnimalDto>.FromData(animal!);
        }
        catch (Exception e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
            return TaskResult<ResponseAnimalDto>.FromFailure("Not animal found with such id: "+id);
        }
    }

    public async Task<TaskResult> PostAnimalAsync(Animal animal)
    {
        var client = _clientFactory.CreateClient("Api");
        var animalJson = JsonSerializer.Serialize(animal);

        using MultipartFormDataContent content = new();

        content.Add(new StringContent(animalJson, Encoding.UTF8, "application/json"), "animalData");

        foreach (var image in animal.Images)
        {
            var safeFileName = WebUtility.HtmlEncode(image.Name);
            var imageContent = new StreamContent(image.OpenReadStream(MaxFileSize));
            imageContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
            content.Add(imageContent, "Images", safeFileName);
        }

        var result = await client.PostAsync("api/animals", content);

        if (result.IsSuccessStatusCode)
            return TaskResult.FromSuccess("Se creo correctamente");
        else
            return TaskResult.FromFailure("Algo salio mal");
    }
}
