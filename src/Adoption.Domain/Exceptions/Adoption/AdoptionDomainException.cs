using System;

namespace Adoption.Domain.Exceptions.Adoption;

public class AdoptionDomainException : DomainException
{
    public AdoptionDomainException(string message) : base(message)
    {
    }
}
