namespace Rideshare.Domain

open System

type DomainError =
    | ValidationError of string
    | BusinessRuleViolation of string
    | NotFound of string


module ValueObjects =
    type Rating = private Rating of int

    module Rating =
        let create (rating: int) =
            if rating < 1 || rating > 5 then
                Error(ValidationError "Rating must be between 1 and 5")
            else
                Ok(Rating rating)

        let value (Rating rating) = rating

    type EmailAddress = private EmailAddress of string

    module Email =

        let create (email: string) =
            if String.IsNullOrWhiteSpace(email) then
                Error(ValidationError "Email cannot be empty")
            elif not (email.Contains("@")) then
                Error(ValidationError "Email must contain @")
            else
                Ok(EmailAddress email)

        let value (EmailAddress email) = email
