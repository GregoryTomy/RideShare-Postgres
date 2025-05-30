namespace Rideshare.Domain

type DomainError =
    | ValidationError of string
    | BusinessRuleViolation of string
    | NotFound of string

module ValueObjects =
    type Rating = private Rating of int
