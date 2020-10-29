namespace CsGenericVisitors
{
    public class Booking
    {
        public string Description { get; private set; }

        public static ValidationResult<Booking, DomainErrors> Create(string description)
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