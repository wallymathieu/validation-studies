using System.Linq;
using Xunit;

namespace CsGenericVisitors
{
    public class ValidatePersonTests
    {
        [Fact]
        public void Given_validPerson()
        {
            var maybePerson = Person.Create(name: "Bob", age: 25, email: "bob@gmail.com",
                bookings: Enumerable.Empty<Booking>());
            Assert.True(maybePerson.IsSuccess);
        }
        [Fact]
        public void Given_badBooking()
        {
            Assert.Equal(
                DomainErrors.DescriptionBetween1And50,
                Booking.Create(description:"" ));
        }
        [Fact]
        public void Given_badName()
        {
            Assert.Equal(DomainErrors.NameBetween1And50,
                Person.Create(name: "", age: 25, email: "bob@gmail.com",bookings:Enumerable.Empty<Booking>()));
        }
        [Fact]
        public void Given_badEmail()
        {
            Assert.Equal(DomainErrors.EmailMustContainAtChar,
                Person.Create(name: "Bob", age: 25, email: "bademail",
                    bookings: Enumerable.Empty<Booking>()));
        }
        [Fact]
        public void Given_badAge()
        {
            Assert.Equal(DomainErrors.AgeBetween0and120,
                Person.Create( name : "Bob", age : 150, email : "bob@gmail.com", bookings: Enumerable.Empty<Booking>() ));
        }
        [Fact]
        public void Given_badEverything()
        {
            Assert.Equal(DomainErrors.NameBetween1And50|DomainErrors.EmailMustContainAtChar|DomainErrors.AgeBetween0and120, 
                Person.Create( name : "", age : 150, email : "bademail", bookings: Enumerable.Empty<Booking>() ));
        }
    }
}
