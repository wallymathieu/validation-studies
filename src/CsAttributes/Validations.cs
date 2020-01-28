using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CsAttributes
{
    class Validations
    {
        public static IEnumerable<ValidationResult> Validate(object o)
        {
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(o, new ValidationContext(o), validationResults, true))
            {
                return validationResults;
            }
            return Enumerable.Empty<ValidationResult>();
        }
        public static string ToString(IEnumerable<ValidationResult> validations)
        {
            return string.Join(";", validations.Select(v =>
                 $"{v.ErrorMessage}:{String.Join(",", v.MemberNames)}"
            ).ToArray());
        }
    }
}
