using System.Linq;
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
            Assert.Equal("DescriptionBetween1And50", Validations.ToString(
                Booking.Create(description:"" )));
        }
        [Fact]
        public void Given_badName()
        {
            Assert.Equal("NameBetween1And50", Validations.ToString(Person.Create(name: "", age: 25, email: "bob@gmail.com",bookings:Enumerable.Empty<Booking>())));
        }
        [Fact]
        public void Given_badEmail()
        {
            Assert.Equal("EmailMustContainAtChar",
                Validations.ToString(Person.Create(name: "Bob", age: 25, email: "bademail",
                    bookings: Enumerable.Empty<Booking>())));
        }
        [Fact]
        public void Given_badAge()
        {
            Assert.Equal("AgeBetween0and120", Validations.ToString(
                Person.Create( name : "Bob", age : 150, email : "bob@gmail.com", bookings: Enumerable.Empty<Booking>() )));
        }
        [Fact]
        public void Given_badEverything()
        {
            Assert.Equal("NameBetween1And50;EmailMustContainAtChar;AgeBetween0and120", 
                Validations.ToString(Person.Create( name : "", age : 150, email : "bademail", bookings: Enumerable.Empty<Booking>() )));
        }
    }
}
