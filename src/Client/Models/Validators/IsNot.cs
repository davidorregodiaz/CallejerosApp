using System;
using System.ComponentModel.DataAnnotations;

namespace Client.Models.Validators;

public class IsNot : ValidationAttribute
{
    private readonly string _valueToCompare;

    public IsNot(string valueToCompare)
    {
        _valueToCompare = valueToCompare;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is string strValue && strValue.Equals(_valueToCompare, StringComparison.OrdinalIgnoreCase))
        {
            return new ValidationResult($"The value cannot be '{_valueToCompare}'.");
        }

        return ValidationResult.Success;
    }

}
