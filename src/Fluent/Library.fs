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
    
    type Name = { unName : String } 
    with static member create s={unName=s}
    type Email = { unEmail : String } 
    with static member create s={unEmail=s}
    type Age = { unAge : int }
    with static member create i={unAge=i}

    type Person = { name : Name
                    email : Email
                    age : Age }
    with static member create name email age={name=name;email=email;age=age }

    type NameValidator()=
        inherit AbstractValidator<Name>()
        do
            base.RuleFor(fun n -> n.unName).MinimumLength(1).MaximumLength(50).WithErrorCode("NameBetween1And50").``👌``
    type EmailValidator()=
        inherit AbstractValidator<Email>()
        do
            base.RuleFor(fun n -> n.unEmail).EmailAddress().WithErrorCode("EmailMustContainAtChar").__
    type AgeValidator()=
        inherit AbstractValidator<Age>()
        do
            base.RuleFor(fun n -> n.unAge).InclusiveBetween(0,120).WithErrorCode("AgeBetween0and120").__
    // Smart constructors
    let mkName s = NameValidator() |> Validations.apply (Name.create s)

    let mkEmail s = EmailValidator() |> Validations.apply (Email.create s)

    let mkAge a = AgeValidator() |> Validations.apply (Age.create a) 
        
    let mkPerson pName pEmail pAge =
        Person.create
        <!> mkName pName
        <*> mkEmail pEmail
        <*> mkAge pAge

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
