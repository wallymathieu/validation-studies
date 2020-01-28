#if INTERACTIVE
#load "../../.paket/load/FSharpPlus.fsx"
#load "../../.paket/load/FluentValidation.fsx"
#else
namespace Fluent
#endif
open System
open FluentValidation
open FSharpPlus
open FSharpPlus.Data

module Validations=
    let apply (v:'T) (validator:#IValidator<'T>) =
        let res = validator.Validate(box v)
        match res.IsValid with
        | true -> Success v
        | false -> Failure (res.Errors |> List.ofSeq)
[<AutoOpen>]
module ValidationsMod=        
    type IRuleBuilder<'T,'Property> with
        member __.__ = ()
        member __.``👌`` = ()
module Person=

    type Person = { name : String
                    email : String
                    age : int }

    type PersonValidator()=
        inherit AbstractValidator<Person>()
        do
            base.RuleFor(fun n -> n.name)
                .MinimumLength(1).WithErrorCode("NameBetween1And50")
                .MaximumLength(50).WithErrorCode("NameBetween1And50")
                .``👌``
            base.RuleFor(fun n -> n.email)
                .EmailAddress().WithErrorCode("EmailMustContainAtChar")
                .__
            base.RuleFor(fun n -> n.age)
                .InclusiveBetween(0,120)
                .WithErrorCode("AgeBetween0and120")
                .__
    // Smart constructor
    let mkPerson pName pEmail pAge =
        let p={name=pName;email=pEmail;age=pAge }
        PersonValidator() |> Validations.apply p

    // Examples

    let validPerson = mkPerson "Bob" "bob@gmail.com" 25
    // Success ({name = {unName = "Bob"}; email = {unEmail = "bob@gmail.com"}; age = {unAge = 25}})

    let badName = mkPerson "" "bob@gmail.com" 25
    // Failure [NameBetween1And50]

    let badEmail = mkPerson "Bob" "bademail" 25
    // Failure [EmailMustContainAtChar]

    let badAge = mkPerson "Bob" "bob@gmail.com" 150
    // Failure [AgeBetween0and120]

    let badEverything = mkPerson "" "bademail" 150
    // Failure [NameBetween1And50;EmailMustContainAtChar;AgeBetween0and120]
