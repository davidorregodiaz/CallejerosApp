

using Core.ValueObjects;
using Shared;

namespace Core.Models;

public class Animal
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    private Breed _breed;
    public Breed Breed => _breed;
    private AnimalType _type;
    public AnimalType Type => _type;
    private readonly List<Requierement> _requierements = new();
    public IReadOnlyCollection<Requierement> Requirements => _requierements.AsReadOnly();

    public TaskResult AddRequierement(string requierement)
    {
        var requierementResult = Requierement.Create(requierement);
        if (requierementResult.IsSuccessful(out var requierementCreated))
        {
            if (_requierements.Count() < 5)
                return TaskResult.FromFailure("You can only add 5 requierements");

            if (!_requierements.Contains(requierementCreated))
            {
                _requierements.Add(requierementCreated);
                return TaskResult.FromSuccess("Requierement added");
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
        var typeResult = Breed.Create(type);
        if (typeResult.IsSuccessful(out var typeCreated))
        {
            _breed = typeCreated;
            return TaskResult.FromSuccess("Type added");
        }
        return TaskResult.FromFailure(typeResult.Message);
    }
}


