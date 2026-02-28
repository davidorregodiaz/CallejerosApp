using System;

namespace Adoption.Domain.Exceptions.Animal;

public class AnimalDomainException : DomainException
{
    public AnimalDomainException(string message) : base(message)
    {
    }
}
