using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CsAttributes
{
    class Validations
    {
        public static IEnumerable<ValidationResult> Validate(Person o)
        {
            var validationResults = new List<ValidationResult>();
            var ivalidationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(o, new ValidationContext(o), ivalidationResults, true))
            {
                validationResults.AddRange(ivalidationResults);
            }
            for (int i = 0; i < o.Bookings.Count; i++)
            {
                var innerValidationResults = new List<ValidationResult>();
                var innerV = o.Bookings[i];
                if (!Validator.TryValidateObject(innerV, new ValidationContext(innerV), innerValidationResults, true))
                {
                    validationResults.AddRange(innerValidationResults.Select(v=>new ValidationResult(v.ErrorMessage,
                        v.MemberNames.Select(mn=>"Bookings["+i+"]."+mn).ToArray())).ToArray());
                }
            }

            return validationResults;
        }
        public static string ToString(IEnumerable<ValidationResult> validations)
        {
            return string.Join(";", validations.Select(v =>
                 $"{v.ErrorMessage}:{String.Join(",", v.MemberNames)}"
            ).ToArray());
        }
    }
}
