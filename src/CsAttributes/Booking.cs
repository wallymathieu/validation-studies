using System.ComponentModel.DataAnnotations;

namespace CsAttributes
{
    public class Booking
    {
        [MinLength(1, ErrorMessage = "DescriptionBetween1And50"), MaxLength(50, ErrorMessage = "DescriptionBetween1And50")]
        public string Description { get; set; }

    }
}