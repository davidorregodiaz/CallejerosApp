// using Adoption.Infrastructure.Context;
// using Microsoft.EntityFrameworkCore;
// using Shared;
// using Shared.Dtos;

// namespace Infrastructure.Services;

// public class AnimalService 
// {

//     private readonly AdoptionDbContext _context;

//     public AnimalService(AdoptionDbContext context)
//     {
//         _context = context;
//     }

//     public async Task<TaskResult<ResponseAnimalDto>> CreateAnimal(CreateAnimalDto animalDto, List<string> picturesPaths)
//     {
//         var result = animalDto.ToModel(null, picturesPaths);

//         if (!result.IsSuccessful(out var animalDb))
//             return TaskResult<ResponseAnimalDto>.FromFailure(result.Message);

//         await _context.Animals.AddAsync(animalDb);
//         await _context.SaveChangesAsync();
//         return TaskResult<ResponseAnimalDto>.FromData(animalDb.ToDto());
//     }

//     public async Task<TaskResult> DeleteAnimal(Guid id, string ownerId)
//     {
//         var animal = await _context.Animals.FindAsync(id);

//         if (animal is null)
//             return TaskResult.FromFailure("Animal not found");

//         if (animal.BelongsTo(ownerId))
//             return TaskResult.FromFailure("You dont have the permissions to delete this animal", 403);

//         _context.Animals.Remove(animal);
//         await _context.SaveChangesAsync();

//         return TaskResult.FromSuccess("Deleted successful");
//     }


//     public async Task<TaskResult<ResponseAnimalDto>> GetAnimalById(Guid id)
//     {
//         var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);

//         if (animal is not null)
//             return TaskResult<ResponseAnimalDto>.FromData(animal.ToDto());

//         return TaskResult<ResponseAnimalDto>.FromFailure("Not found");
//     }

//     public async Task<TaskResult<IEnumerable<ResponseAnimalDto>>> FilterAnimalsByBreed(string breed)
//     {
//         var animalsWithBreed = await _context.Animals.Where(a => a.Breed!.Value == breed).ToListAsync();

//         if (!animalsWithBreed.Any())
//             return TaskResult<IEnumerable<ResponseAnimalDto>>.FromFailure("Not found");

//         return TaskResult<IEnumerable<ResponseAnimalDto>>.FromData(animalsWithBreed.Select(a => a.ToDto()));
//     }

//     public async Task<TaskResult<IEnumerable<ResponseAnimalDto>>> FilterAnimalsByType(string type)
//     {
//         var animalsWithType = await _context.Animals.Where(a => a.AnimalType!.Value == type).ToListAsync();

//         if (!animalsWithType.Any())
//             return TaskResult<IEnumerable<ResponseAnimalDto>>.FromFailure("Not found");

//         return TaskResult<IEnumerable<ResponseAnimalDto>>.FromData(animalsWithType.Select(a => a.ToDto()));
//     }

//     public async Task<TaskResult<IEnumerable<ResponseAnimalDto>>> FilterAnimalsByOwnerId(string ownerId)
//     {
//         var animals = await _context.Animals.Where(a => a.OwnerId == Guid.Parse(ownerId)).ToListAsync();

//         if (!animals.Any())
//             return TaskResult<IEnumerable<ResponseAnimalDto>>.FromFailure("No animals found for this owner");

//         return TaskResult<IEnumerable<ResponseAnimalDto>>.FromData(animals.Select(a => a.ToDto()));
//     }

//     public async Task<TaskResult<IEnumerable<ResponseAnimalDto>>> FilterAnimalsByTypeAndBreed(string type, string breed)
//     {
//         if (string.IsNullOrWhiteSpace(type))
//             return TaskResult<IEnumerable<ResponseAnimalDto>>.FromFailure("Type parameter must not be empty");

//         if (string.IsNullOrWhiteSpace(breed))
//             return TaskResult<IEnumerable<ResponseAnimalDto>>.FromFailure("Breed parameter must not be empty");

//         var animals = await _context.Animals
//             .Where(a => a.AnimalType!.Value == type)
//             .Where(a => a.Breed!.Value == breed)
//             .ToListAsync();

//         if (!animals.Any())
//             return TaskResult<IEnumerable<ResponseAnimalDto>>.FromFailure("No animals found with that type and breed");

//         return TaskResult<IEnumerable<ResponseAnimalDto>>.FromData(animals.Select(a => a.ToDto()));
//     }


//     public async Task<TaskResult<IEnumerable<ResponseAnimalDto>>> GetAnimals()
//     {
//         if (!await _context.Animals.AnyAsync())
//             return TaskResult<IEnumerable<ResponseAnimalDto>>.FromFailure("No animals persisted");

//         var animals = await _context.Animals.ToListAsync();
//         return TaskResult<IEnumerable<ResponseAnimalDto>>.FromData(animals.Select(a => a.ToDto()));
//     }

//     public async Task<TaskResult<ResponseAnimalDto>> UpdateAnimal(CreateAnimalDto animalDto, Guid id, string ownerId)
//     {
//         var animalDb = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id && a.OwnerId == Guid.Parse(ownerId));

//         if (animalDb is not null)
//         {
//             var result = animalDto.ToModel(animalDb, null);

//             if (!result.IsSuccessful(out var animalUpdated))
//                 return TaskResult<ResponseAnimalDto>.FromFailure(result.Message);

//             if (!animalUpdated.BelongsTo(ownerId))
//                 return TaskResult<ResponseAnimalDto>.FromFailure("You dont have the permissions to delete this animal", 405);

//             _context.Update(animalUpdated);
//             await _context.SaveChangesAsync();
//             return TaskResult<ResponseAnimalDto>.FromData(animalUpdated.ToDto());
//         }

//         return TaskResult<ResponseAnimalDto>.FromFailure("Not found");
//     }
// }

