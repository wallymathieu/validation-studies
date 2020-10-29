using System.Collections.Generic;

namespace CsGenericVisitors
{
    public class Person
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public int Age { get; private set;}
        public IEnumerable<Booking> Bookings { get; private set;}

        public static ValidationResult<Person, DomainErrors> Create(string name, string email, int age, IEnumerable<Booking> bookings)
        {
            var errors = DomainErrors.None;
            if (name is null || 1 > name.Length || name.Length > 50)
            {
                errors |= DomainErrors.NameBetween1And50;
            }

            if (email is null || !email.Contains("@"))
            {
                errors|=DomainErrors.EmailMustContainAtChar;
            }

            if (0 > age || age > 120)
            {
                errors|=DomainErrors.AgeBetween0and120;
            }

            if (errors!=DomainErrors.None)
                return errors;
            else
                return new Person
                {
                    Bookings = bookings,
                    Name = name,
                    Email = email,
                    Age = age
                };
        }
    }
}
