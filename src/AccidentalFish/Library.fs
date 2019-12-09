#if INTERACTIVE
#load "../../.paket/load/FSharpPlus.fsx"
#load "../../.paket/load/AccidentalFish.FSharp.Validation.fsx"
#else
namespace AccidentalFish
#endif
open System
open FSharpPlus
open FSharpPlus.Data
open AccidentalFish.FSharp.Validation

module Person=
    type Person = { name : string
                    email : string
                    age : int }
    with static member create name email age={name=name;email=email;age=age }
    let containsAt (s:string)= s.Contains("@")
    let validatePerson = createValidatorFor<Person>() {
        validate (fun r -> r.age) [
            isGreaterThanOrEqualTo 0
            isLessThanOrEqualTo 120
        ] //AgeBetween0and120
        validate (fun r -> r.name) [
            isNotEmpty
            hasMaxLengthOf 128
            hasMinLengthOf 1
        ] //NameBetween1And50
        validate (fun r -> r.email) [
            (fun p v -> if (containsAt v) then Ok else Errors([{ message="EmailMustContainAtChar"; errorCode="EmailMustContainAtChar"; property=p }]))
        ]
    }
    // Smart constructors
    let mkPerson pName pEmail pAge =
        let p = Person.create pName pEmail pAge
        let res = validatePerson p
        match res with
        | Ok -> Success p
        | Errors f -> Failure f

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
