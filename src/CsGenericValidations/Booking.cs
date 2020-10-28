using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CsGenericValidations
{
    public class Booking
    {
        public string Description { get; private set; }

        private class ValidationResult:ValidationResult<Booking, ReadOnlyCollection<DomainErrors>>
        {
        }

        public static ValidationResult<Booking, ReadOnlyCollection<DomainErrors>> Create(string description)
        {
            var errors = new List<DomainErrors>();
            if (description is null || 1 > description.Length || description.Length > 50)
            {
                errors.Add(DomainErrors.DescriptionBetween1And50);
            }

            return errors.Any() 
                ? ValidationResult
                    .NewFailure(errors.AsReadOnly()) 
                : ValidationResult
                    .NewSuccess(new Booking { Description = description });
        }
    }
}