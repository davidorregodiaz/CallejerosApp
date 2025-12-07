using Adoption.API.Application.Commands.AdoptionRequests;
using FluentValidation;

namespace Adoption.API.Application.Validators;

public class CreateAdoptionRequestCommandValidator : AbstractValidator<CreateAdoptionRequestCommand>
{
    public CreateAdoptionRequestCommandValidator()
    {
        RuleFor(x => x.Comments)
            .NotEmpty()
            .WithMessage("Comments cannot be empty")
            .MaximumLength(200)
            .WithMessage("Comments must not exceed 200 characters");;
    }
}
