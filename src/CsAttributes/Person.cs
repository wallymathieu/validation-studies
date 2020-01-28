using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CsAttributes
{
    public class Person
    {
        [MinLength(1,ErrorMessage ="NameBetween1And50"), MaxLength(50,ErrorMessage ="NameBetween1And50")]
        public string Name{get;set;}
        [EmailAddress(ErrorMessage ="EmailMustContainAtChar")]
        public string Email{get;set;}
        [Range(0,120,ErrorMessage = "AgeBetween0and120")]
        public int Age {get;set;}
        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
