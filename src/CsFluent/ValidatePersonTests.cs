using Xunit;

namespace CsFluent
{
    public class ValidatePersonTests
    {
        [Fact]
        public void Given_validPerson()
        {
            Assert.True(Validations.Validate(new Person { Name = "Bob", Age = 25, Email = "bob@gmail.com" }).IsValid);
        }
        [Fact]
        public void Given_badBooking()
        {
            Assert.Equal("DescriptionBetween1And50:Bookings[0].Description", Validations.ToString(Validations.Validate(
                new Person { Name = "Bob", Age = 25, Email = "bob@gmail.com", Bookings = { new Booking { Description="" } } })));
        }
        [Fact]
        public void Given_badName()
        {
            Assert.Equal("NameBetween1And50:Name", Validations.ToString(Validations.Validate(new Person { Name = "", Age = 25, Email = "bob@gmail.com" })));
        }
        [Fact]
        public void Given_badEmail()
        {
            Assert.Equal("EmailMustContainAtChar:Email", Validations.ToString(Validations.Validate(new Person { Name = "Bob", Age = 25, Email = "bademail" })));
        }
        [Fact]
        public void Given_badAge()
        {
            Assert.Equal("AgeBetween0and120:Age", Validations.ToString(Validations.Validate(new Person { Name = "Bob", Age = 150, Email = "bob@gmail.com" })));
        }
        [Fact]
        public void Given_badEverything()
        {
            Assert.Equal("NameBetween1And50:Name;EmailMustContainAtChar:Email;AgeBetween0and120:Age", Validations.ToString(Validations.Validate(new Person { Name = "", Age = 150, Email = "bademail" })));
        }
    }
}
