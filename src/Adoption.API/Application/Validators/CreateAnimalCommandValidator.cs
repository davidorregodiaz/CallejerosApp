using Adoption.API.Application.Commands.Animals;
using FluentValidation;

namespace Adoption.API.Application.Validators;

public class CreateAnimalCommandValidator : AbstractValidator<CreateAnimalCommand>
{
    public CreateAnimalCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MinimumLength(3)
            .WithMessage("Name must be at least 3 characters long")
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters");
        
        RuleFor(x => x.Breed)
            .NotEmpty()
            .WithMessage("Breed is required")
            .MinimumLength(3)
            .WithMessage("Breed must be at least 3 characters long")
            .MaximumLength(100)
            .WithMessage("Breed cannot exceed 100 characters");
        
        RuleFor(x => x.Species)
            .NotEmpty()
            .WithMessage("Species is required")
            .MinimumLength(3)
            .WithMessage("Species must be at least 3 characters long")
            .MaximumLength(100)
            .WithMessage("Species cannot exceed 100 characters");

        RuleFor(x => x.OwnerId)
            .NotEmpty()
            .WithMessage("OwnerId is required");
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MinimumLength(3)
            .WithMessage("Description must be at least 3 characters long")
            .MaximumLength(300)
            .WithMessage("Description cannot exceed 300 characters");
    }
}
