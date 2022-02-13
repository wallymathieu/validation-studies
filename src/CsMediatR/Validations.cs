﻿using FluentValidation.Results;

namespace CsMediatR;

class Validations
{
    public static ValidationResult Validate(Person o)
    {
        var pv = new PersonValidator();
        return pv.Validate(o);
    }
    public static string ToString(ValidationResult validations)
    {
        return string.Join(";", validations.Errors.Select(v =>
            $"{v.ErrorCode}:{String.Join(",", v.PropertyName)}"
        ).ToArray());
    }
}