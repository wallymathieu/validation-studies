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
    type Person = { [<MinLength(1,ErrorMessage ="NameBetween1And50"); MaxLength(50,ErrorMessage ="NameBetween1And50")>]name : string
                    [<EmailAddress(ErrorMessage ="EmailMustContainAtChar")>]email : string
                    [<Range(0,120,ErrorMessage = "AgeBetween0and120")>]age : int }
    with static member create name email age={name=name;email=email;age=age }

    // Smart constructors
    let mkPerson pName pEmail pAge =
        let p=Person.create pName pEmail pAge
        Validations.apply p

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
