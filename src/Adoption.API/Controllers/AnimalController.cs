using System.Text.Json;
using Adoption.API.Application.Commands.Animals;
using Adoption.API.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;

namespace Adoption.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{

    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IAnimalQueries _animalQueries;
    public AnimalsController(IWebHostEnvironment webHostEnvironment, IAnimalQueries animalQueries)
    {
        _webHostEnvironment = webHostEnvironment;
        _animalQueries = animalQueries;
    }

    private List<string> ProcessImages(IEnumerable<IFormFile> images)
    {
        var uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images", "upload");

        if (!Directory.Exists(uploadDir))
        {
            Directory.CreateDirectory(uploadDir);
        }

        var picturesPaths = new List<string>();

        foreach (var file in images)
        {
            string safeFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var path = Path.Combine(uploadDir, safeFileName);
            picturesPaths.Add(path);
            using var stream = new FileStream(path, FileMode.Create);
            file.CopyTo(stream);
        }
        return picturesPaths;
    }

    public static string ConvertToWebPath(string physicalPath)
    {
        if (string.IsNullOrEmpty(physicalPath))
            return string.Empty;
            
        var basePhysicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        
        if (physicalPath.StartsWith(basePhysicalPath))
        {
            return physicalPath
                .Substring(basePhysicalPath.Length)
                .Replace('\\', '/');
        }
        
        return physicalPath;
    }

    [HttpPost]
    [Authorize]
    public async Task<IResult> CreateAnimal([FromForm] string animalData,[FromForm] IEnumerable<IFormFile> images)
    {
        var createAnimalDto = JsonSerializer.Deserialize<CreateAnimalDto>(animalData);

        if (createAnimalDto is null)
            return Results.BadRequest("Invalid animal data");

        var picturesPaths = ProcessImages(images);

        var createAnimalCommand = new CreateAnimalCommand(
            createAnimalDto.OwnerId,
            createAnimalDto.Name,
            createAnimalDto.Age,
            createAnimalDto.Breed,
            createAnimalDto.Type,
            createAnimalDto.Description,
            picturesPaths.Select(ConvertToWebPath).ToList()
        );

        // var createAnimalResult = await _mediator.Send(createAnimalCommand); //TODO Here is where it sends the command through mediator

        // if (!createAnimalResult.IsSuccessful(out var animalCreated))
        // {
        //     return Results.BadRequest(createAnimalResult.Message);
        // }
        return Results.CreatedAtRoute("GetAnimalById", new { id = 2 });
    }

    [HttpGet("{id}", Name = "GetAnimalById")]
    public async Task<IResult> GetAnimalById(Guid id)
    {
        var result = await _animalQueries.FindAnimalById(id);

        if (!result.IsSuccessful(out var animal))
        {
            return Results.NotFound(result.Message);
        }

        return Results.Ok(animal);
    }

    [HttpGet]
    public async Task<IResult> GetAnimals()
    {
        var result = await _animalQueries.FindAllAnimals();
        if (!result.IsSuccessful(out var animals))
        {
            return Results.NotFound(result.Message);
        }
        return Results.Ok(animals);
    }

    // [HttpGet("filter/by-breed")]
    // public async Task<IResult> FilterAnimalsByBreed([FromQuery] string breed)
    // {
    //     var result = await _animalService.FilterAnimalsByBreed(breed);
    //     if (!result.IsSuccessful(out var animalsByBreed))
    //     {
    //         return Results.NotFound(result.Message);
    //     }
    //     return Results.Ok(animalsByBreed);
    // }

    // [HttpGet("filter/by-type")]
    // public async Task<IResult> FilterAnimalsByType([FromQuery] string type)
    // {
    //     var result = await _animalService.FilterAnimalsByType(type);
    //     if (!result.IsSuccessful(out var animalsByType))
    //     {
    //         return Results.NotFound(result.Message);
    //     }
    //     return Results.Ok(animalsByType);
    // }

    // [HttpGet("filter/by-type-and-breed")]
    // public async Task<IResult> FilterAnimalsByTypeAndBreed([FromQuery] string type, [FromQuery] string breed)
    // {
    //     var result = await _animalService.FilterAnimalsByTypeAndBreed(type, breed);
    //     if (!result.IsSuccessful(out var animalsByTypeAndBreed))
    //     {
    //         return Results.NotFound(result.Message);
    //     }
    //     return Results.Ok(animalsByTypeAndBreed);
    // }

    // [HttpGet("filter/by-owner")]
    // public async Task<IResult> FilterAnimalsByOwnerId([FromQuery] string ownerId)
    // {
    //     var result = await _animalService.FilterAnimalsByOwnerId(ownerId);
    //     if (!result.IsSuccessful(out var animalsByOwner))
    //     {
    //         return Results.NotFound(result.Message);
    //     }
    //     return Results.Ok(animalsByOwner);
    // }

    // [HttpDelete("{id}")]
    // [Authorize]
    // public async Task<IResult> DeleteAnimal(Guid id)
    // {
    //     var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //     if (string.IsNullOrEmpty(ownerId))
    //         return Results.Unauthorized();

    //     var result = await _animalService.DeleteAnimal(id, ownerId);
    //     if (!result.Success)
    //     {
    //         if (result.Code == 403)
    //             return Results.Forbid();
    //         return Results.NotFound(result.Message);
    //     }
    //     return Results.Ok(result.Message);
    // }

    // [HttpPut("{id}")]
    // [Authorize]
    // public async Task<IResult> UpdateAnimal([FromBody] CreateAnimalDto animalDto, Guid id)
    // {
    //     var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //     if (string.IsNullOrEmpty(ownerId))
    //         return Results.Unauthorized();

    //     var updateAnimalResult = await _animalService.UpdateAnimal(animalDto, id, ownerId);
    //     if (!updateAnimalResult.IsSuccessful(out var animalUpdated))
    //     {
    //         return Results.BadRequest(updateAnimalResult.Message);
    //     }
    //     return Results.CreatedAtRoute("GetAnimalById", new { id = animalUpdated.Id }, animalUpdated);
    // }
}
