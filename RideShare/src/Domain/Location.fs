namespace Rideshare.Domain

open System
open ValueObjects

module Location =
    type LocationId = LocationId of Guid
    type Address = private Address of string

    module Address =
        let create (address: string) =
            if String.IsNullOrWhiteSpace(address) then
                Error(ValidationError "Address cannot be empty")
            elif address.Length > 255 then
                Error(ValidationError "Address cannot exceed 255 characters")
            else
                Ok(Address address)

        let value (Address address) = address

    type StateCode = private StateCode of string

    module StateCode =
        let create (stateCode: string) =
            if String.IsNullOrWhiteSpace(stateCode) then
                Error(ValidationError "State Code cannot be empty")
            elif stateCode.Length <> 2 then
                Error(ValidationError "State Code must be exactly 2 characters")
            else
                Ok(StateCode stateCode)

        let value (StateCode stateCode) = stateCode

    type Coordinates =
        { Latitude: decimal
          Longitude: decimal }

    module Coordinates =
        let create (latitude: decimal) (longitude: decimal) =
            if latitude < -90m || latitude > 90m then
                Error(ValidationError "Latitude must be between -90 and 90")
            elif longitude < -180m || longitude > 180m then
                Error(ValidationError "Longitude must be between -180 and 180")
            else
                Ok
                    { Latitude = latitude
                      Longitude = longitude }


    type Location =
        { Id: LocationId
          Address: Address
          City: string option
          State: StateCode
          Position: Coordinates
          CreatedAt: DateTimeOffset
          UpdatedAt: DateTimeOffset }

    let createLocation address city state latitude longitude =
        match Address.create address with
        | Error err -> Error err
        | Ok validAddress ->
            match StateCode.create state with
            | Error err -> Error err
            | Ok validStateCode ->
                match Coordinates.create latitude longitude with
                | Error err -> Error err
                | Ok validCoordinates ->
                    Ok
                        { Id = LocationId(Guid.NewGuid())
                          Address = validAddress
                          City = city
                          State = validStateCode
                          Position = validCoordinates
                          CreatedAt = DateTimeOffset.Now
                          UpdatedAt = DateTimeOffset.Now }

    let getAddressa location = Address.value location.Address
    let getState location = StateCode.value location.State
    let getCoordinates location = location.Position
