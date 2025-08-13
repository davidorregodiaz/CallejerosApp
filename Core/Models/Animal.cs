

using System.Text.Json.Serialization;
using Core.ValueObjects;
using Shared;

namespace Core.Models;

public class Animal
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int Age { get; set; }
    private Breed? _breed;
    public Breed? Breed => _breed;
    private AnimalType? _type;
    public AnimalType? AnimalType => _type;
    private readonly List<Requierement> _requierements = new();
    public IReadOnlyCollection<Requierement> Requirements => _requierements.AsReadOnly();
    public AppUser Owner { get; set; } 
    public string OwnerId { get; set; }
    
    public ICollection<string> ImagesPath { get; set; } = new List<string>();

    public bool BelongsTo(string userId) => OwnerId == userId;

    public TaskResult AddRequierement(string requierement)
    {
        if (_requierements.Count >= 5)
            return TaskResult.FromFailure("You can only add 5 requierements");

        var requierementResult = Requierement.Create(requierement);
        if (requierementResult.IsSuccessful(out var requierementCreated))
        {
            if (!_requierements.Contains(requierementCreated))
            {
                _requierements.Add(requierementCreated);
                Console.WriteLine(_requierements.Count);
                return TaskResult.FromSuccess("Requierement added");
            }
            else
            {
                return TaskResult.FromFailure("Requierement already exists");
            }
        }
        return TaskResult.FromFailure(requierementResult.Message);
    }

    public TaskResult RemoveRequierement(string requierement)
    {
        var requierementResult = Requierement.Create(requierement);
        if (requierementResult.IsSuccessful(out var requierementCreated))
        {
            if (_requierements.Contains(requierementCreated))
            {
                _requierements.Remove(requierementCreated);
                return TaskResult.FromSuccess("Requierement deleted");
            }
            return TaskResult.FromFailure("Requierement not found");
        }
        return TaskResult.FromFailure(requierementResult.Message);
    }

    public void ClearRequierements()
    {
        _requierements.Clear();
    }


    public TaskResult SetBreed(string breed)
    {
        var breedResult = Breed.Create(breed);
        if (breedResult.IsSuccessful(out var breedCreated))
        {
            _breed = breedCreated;
            return TaskResult.FromSuccess("Breed added");
        }
        return TaskResult.FromFailure(breedResult.Message);
    }

     public TaskResult SetType(string type)
    {
        var typeResult = AnimalType.Create(type);
        if (typeResult.IsSuccessful(out var typeCreated))
        {
            _type = typeCreated;
            return TaskResult.FromSuccess("Type added");
        }
        return TaskResult.FromFailure(typeResult.Message);
    }
}


