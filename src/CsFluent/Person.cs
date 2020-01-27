using System;
using FluentValidation;

namespace CsFluent
{
    public class Person
    {
        public string Name{get;set;}
        public string Email{get;set;}
        public int Age {get;set;}
    }
    public class PersonValidator:AbstractValidator<Person>{
        public PersonValidator(){
            RuleFor(n => n.Name).MinimumLength(1).MaximumLength(50).WithErrorCode("NameBetween1And50");
            RuleFor(n => n.Email).EmailAddress().WithErrorCode("EmailMustContainAtChar");
            RuleFor(n => n.Age).InclusiveBetween(0,120).WithErrorCode("AgeBetween0and120");
        }
    }
}
