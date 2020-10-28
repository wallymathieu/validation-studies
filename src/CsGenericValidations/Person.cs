using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CsGenericValidations
{
    public class Person
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public int Age { get; private set;}
        public IEnumerable<Booking> Bookings { get; private set;}
        private class ValidationResult:ValidationResult<Person, ReadOnlyCollection<DomainErrors>>
        {
        }

        public static ValidationResult<Person, ReadOnlyCollection<DomainErrors>> Create(string name, string email, int age, IEnumerable<Booking> bookings)
        {
            var errors = new List<DomainErrors>();
            if (name is null || 1 > name.Length || name.Length > 50)
            {
                errors.Add(DomainErrors.NameBetween1And50);
            }

            if (email is null || !email.Contains("@"))
            {
                errors.Add(DomainErrors.EmailMustContainAtChar);
            }

            if (0 > age || age > 120)
            {
                errors.Add(DomainErrors.AgeBetween0and120);
            }

            return errors.Any() 
                ? ValidationResult
                    .NewFailure(errors.AsReadOnly()) 
                : ValidationResult
                    .NewSuccess(new Person{
                        Bookings= bookings,
                        Name= name,
                        Email= email,
                        Age= age});
        }
    }
}
