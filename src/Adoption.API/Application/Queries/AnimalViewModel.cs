using System;

namespace Adoption.API.Application.Queries;


    public record Animal(
        Guid Id,
        Guid OwnerId,
        string Name,
        int Age,
        string Breed,
        string Type,
        IReadOnlyCollection<string> Images,
        string Description
    );


