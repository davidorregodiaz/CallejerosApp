using System.Security.Claims;
using Application.Dtos;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {

        private readonly IAnimalService _animalService;

        public AnimalsController(IAnimalService animalService)
        {
            _animalService = animalService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IResult> CreateAnimal([FromBody] CreateAnimalDto animalDto)
        {
            var createAnimalResult = await _animalService.CreateAnimal(animalDto);
            if (!createAnimalResult.IsSuccessful(out var animalCreated))
            {
                return Results.BadRequest(createAnimalResult.Message);
            }
            return Results.CreatedAtRoute("GetAnimalById", new { id = animalCreated.Id }, animalCreated);
        }

        [HttpGet("{id}", Name = "GetAnimalById")]
        public async Task<IResult> GetAnimalById(Guid id)
        {
            var result = await _animalService.GetAnimalById(id);

            if (!result.IsSuccessful(out var animal))
            {
                return Results.NotFound(result.Message);
            }

            return Results.Ok(animal);
        }

        [HttpGet]
        public async Task<IResult> GetAnimals()
        {
            var result = await _animalService.GetAnimals();
            if (!result.IsSuccessful(out var animals))
            {
                return Results.NotFound(result.Message);
            }
            return Results.Ok(animals);
        }

        [HttpGet("filter/by-breed")]
        public async Task<IResult> FilterAnimalsByBreed([FromQuery] string breed)
        {
            var result = await _animalService.FilterAnimalsByBreed(breed);
            if (!result.IsSuccessful(out var animalsByBreed))
            {
                return Results.NotFound(result.Message);
            }
            return Results.Ok(animalsByBreed);
        }

        [HttpGet("filter/by-type")]
        public async Task<IResult> FilterAnimalsByType([FromQuery] string type)
        {
            var result = await _animalService.FilterAnimalsByType(type);
            if (!result.IsSuccessful(out var animalsByType))
            {
                return Results.NotFound(result.Message);
            }
            return Results.Ok(animalsByType);
        }

        [HttpGet("filter/by-type-and-breed")]
        public async Task<IResult> FilterAnimalsByTypeAndBreed([FromQuery] string type, [FromQuery] string breed)
        {
            var result = await _animalService.FilterAnimalsByTypeAndBreed(type, breed);
            if (!result.IsSuccessful(out var animalsByTypeAndBreed))
            {
                return Results.NotFound(result.Message);
            }
            return Results.Ok(animalsByTypeAndBreed);
        }

        [HttpGet("filter/by-owner")]
        public async Task<IResult> FilterAnimalsByOwnerId([FromQuery] string ownerId)
        {
            var result = await _animalService.FilterAnimalsByOwnerId(ownerId);
            if (!result.IsSuccessful(out var animalsByOwner))
            {
                return Results.NotFound(result.Message);
            }
            return Results.Ok(animalsByOwner);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IResult> DeleteAnimal(Guid id)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(ownerId))
                return Results.Unauthorized();

            var result = await _animalService.DeleteAnimal(id, ownerId);
            if (!result.Success)
            {
                if (result.Code == 403)
                    return Results.Forbid();
                return Results.NotFound(result.Message);
            }
            return Results.Ok(result.Message);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IResult> UpdateAnimal([FromBody] CreateAnimalDto animalDto, Guid id)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(ownerId))
                return Results.Unauthorized();

            var updateAnimalResult = await _animalService.UpdateAnimal(animalDto, id, ownerId);
            if (!updateAnimalResult.IsSuccessful(out var animalUpdated))
            {
                return Results.BadRequest(updateAnimalResult.Message);
            }
            return Results.CreatedAtRoute("GetAnimalById", new { id = animalUpdated.Id }, animalUpdated);
        }
    }
}
