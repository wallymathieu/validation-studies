#if INTERACTIVE
#load "../../.paket/load/FSharpPlus.fsx"
#load "../../.paket/load/System.ComponentModel.Annotations.fsx"
#else
namespace AttributeBasedValidations
#endif


open System.ComponentModel.DataAnnotations
open FSharpPlus
open FSharpPlus.Data

module Validations=
    let apply v=
        let err = ResizeArray()
        match Validator.TryValidateObject (v, ValidationContext (v), err, true) with
        | true -> Success v
        | false -> Failure (err |> List.ofSeq)

module Person=
    
    type Name = { [<MinLength(1,ErrorMessage ="NameBetween1And50"); MaxLength(50,ErrorMessage ="NameBetween1And50")>]unName : string } 
    with static member create s={unName=s}
    type Email = { [<EmailAddress(ErrorMessage ="EmailMustContainAtChar")>]unEmail : string } 
    with static member create s={unEmail=s}
    type Age = { [<Range(0,120,ErrorMessage = "AgeBetween0and120")>]unAge : int }
    with static member create i={unAge=i}

    type Person = { name : Name
                    email : Email
                    age : Age }
    with static member create name email age={name=name;email=email;age=age }



    // Smart constructors
    let mkName s = Name.create s |> Validations.apply

    let mkEmail s =  Email.create s |> Validations.apply

    let mkAge a = Age.create a |> Validations.apply
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
