using System;

namespace Adoption.Domain.Exceptions.Adoption;

public class AdoptionDomainException : DomainException
{
    public AdoptionDomainException() : base() { }

    public AdoptionDomainException(string message) : base(message) { }
    public AdoptionDomainException(string message, Exception innerException) 
        : base(message, innerException) { }
}
