using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using OneOf;
using Xunit;

namespace CsGenericValidations
{
    public class ValidatePersonTests
    {
        [Fact]
        public void Given_validPerson()
        {
            Assert.True(Person.Create( name : "Bob", age : 25, email : "bob@gmail.com", bookings: Enumerable.Empty<Booking>() ).IsValid);
        }
        [Fact]
        public void Given_badBooking()
        {
            Assert.Equal("DescriptionBetween1And50", ToString(
                Booking.Create(description:"" )));
        }
        [Fact]
        public void Given_badName()
        {
            Assert.Equal("NameBetween1And50", ToString(Person.Create(name: "", age: 25, email: "bob@gmail.com",bookings:Enumerable.Empty<Booking>())));
        }
        [Fact]
        public void Given_badEmail()
        {
            Assert.Equal("EmailMustContainAtChar",
                ToString(Person.Create(name: "Bob", age: 25, email: "bademail",
                    bookings: Enumerable.Empty<Booking>())));
        }
        [Fact]
        public void Given_badAge()
        {
            Assert.Equal("AgeBetween0and120", ToString(
                Person.Create( name : "Bob", age : 150, email : "bob@gmail.com", bookings: Enumerable.Empty<Booking>() )));
        }
        [Fact]
        public void Given_badEverything()
        {
            Assert.Equal(ToString(ValidationResult<Person,ReadOnlyCollection< DomainErrors>>
                    .NewFailure(new []
                        {
                            DomainErrors.NameBetween1And50,DomainErrors.EmailMustContainAtChar,DomainErrors.AgeBetween0and120
                        }.ToList().AsReadOnly())), 
                ToString(Person.Create( name : "", age : 150, email : "bademail", bookings: Enumerable.Empty<Booking>() )));
        }

        private static string ToString<TSuccess,TFailure>(OneOfBase<ValidationResult<TSuccess, TFailure>.Success, ValidationResult<TSuccess, TFailure>.Failure> validations)
        {
            return validations.Match<string>(success => "Success", failure =>
                failure.Value is IEnumerable
                    ? string.Join(";", ValuesToString((IEnumerable)failure.Value))
                    : failure.Value.ToString()
            );
        }

        private static IEnumerable<string> ValuesToString(IEnumerable enumerable )
        {
            return from f in enumerable.Cast<object>() select f.ToString();
        }
    }
}
