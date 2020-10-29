using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OneOf;

namespace CsGenericValidations
{
    public class Booking
    {
        public string Description { get; private set; }

        public static OneOf<Booking, DomainErrors> Create(string description)
        {
            var errors = DomainErrors.None;
            if (description is null || 1 > description.Length || description.Length > 50)
            {
                errors|=DomainErrors.DescriptionBetween1And50;
            }

            if (errors!=DomainErrors.None)
                return errors;
            else
                return new Booking {Description = description};
        }
    }
}