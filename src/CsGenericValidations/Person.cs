using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CsGenericValidations
{
    public enum SampleErrors
    {
        None=0,
        NameBetween1And50,
        EmailMustContainAtChar,
        AgeBetween0and120,
        DescriptionBetween1And50,
    }

    public class Person
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public int Age { get; private set;}
        public IEnumerable<Booking> Bookings { get; private set;}


        public static ValidationResult<Person, ReadOnlyCollection<SampleErrors>> Create(string name, string email, int age, IEnumerable<Booking> bookings)
        {
            var errors = new List<SampleErrors>();
            if (name is null || 1 > name.Length || name.Length > 50)
            {
                errors.Add(SampleErrors.NameBetween1And50);
            }

            if (email is null || !email.Contains("@"))
            {
                errors.Add(SampleErrors.EmailMustContainAtChar);
            }

            if (0 > age || age > 120)
            {
                errors.Add(SampleErrors.AgeBetween0and120);
            }

            return errors.Any() 
                ? ValidationResult<Person, ReadOnlyCollection<SampleErrors>>
                    .NewFailure(errors.AsReadOnly()) 
                : ValidationResult<Person, ReadOnlyCollection<SampleErrors>>
                    .NewSuccess(new Person{
                        Bookings= bookings,
                        Name= name,
                        Email= email,
                        Age= age});
        }
    }
    public class Booking
    {
        public string Description { get; private set; }


        public static ValidationResult<Booking, ReadOnlyCollection<SampleErrors>> Create(string description)
        {
            var errors = new List<SampleErrors>();
            if (description is null || 1 > description.Length || description.Length > 50)
            {
                errors.Add(SampleErrors.DescriptionBetween1And50);
            }

            return errors.Any() 
                ? ValidationResult<Booking, ReadOnlyCollection<SampleErrors>>
                    .NewFailure(errors.AsReadOnly()) 
                : ValidationResult<Booking, ReadOnlyCollection<SampleErrors>>
                    .NewSuccess(new Booking { Description = description });
        }
    }
}
