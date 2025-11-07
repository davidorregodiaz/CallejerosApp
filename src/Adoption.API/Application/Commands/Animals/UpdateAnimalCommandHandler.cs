using Adoption.API.Abstractions;
using Adoption.API.Application.Mappers;
using Adoption.API.Application.Models;
using Adoption.API.Application.Services;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Adoption.API.Application.Commands.Animals;

public class UpdateAnimalCommandHandler(AdoptionDbContext ctx, IMinioService minioService) : ICommandHandler<UpdateAnimalCommand,  AnimalResponse>
{
    public async Task<AnimalResponse> HandleAsync(UpdateAnimalCommand command, CancellationToken cancellationToken)
    {
        var animal =
            await ctx.Animals.SingleOrDefaultAsync(x => x.Id == new AnimalId(command.AnimalId), cancellationToken);

        if (animal is null)
            throw new ApplicationException($"No animal with id - {command.AnimalId} found");

        var principalImagePresignedUrl = await minioService.UploadFileAsync(command.PrincipalImage);

        var aditionalImagesPresignedUrls = new List<string>();

        if (command.AdditionalImages != null)
        {
            foreach (var additionalImage in command.AdditionalImages)
            {
                aditionalImagesPresignedUrls.Add(await minioService.UploadFileAsync(additionalImage));
            }
        }
        
        animal.Update(
            name: command.Name ?? animal.Name,
            age: command.Age ?? animal.Age,
            description: command.Description ?? animal.Description,
            breed: command.Breed ?? animal.Breed,
            species: command.Species ?? animal.Species,
            principalImage: principalImagePresignedUrl ?? animal.PrincipalImage,
            aditionalImages: aditionalImagesPresignedUrls.Any() ? aditionalImagesPresignedUrls : animal.AdditionalImagesUrl?.ToList()
        );

        await ctx.SaveChangesAsync(cancellationToken);

        return animal.MapToResponse();
    }
}
