namespace Rideshare.Domain

open System
open ValueObjects

module User =
    type UserId = UserId of Guid

    type DriverInfo =
        { LicenseNumber: string
          VehicleInfo: string option }

    type UserType =
        | Driver of DriverInfo
        | Rider

    type User =
        { Id: UserId
          FirstName: string
          LastName: string
          Email: EmailAddress
          UserType: UserType
          CreatedAt: DateTimeOffset
          UpdatedAt: DateTimeOffset }

    let createUser firstName lastName email userType =
        match Email.create email with
        | Ok validEmail ->
            Ok
                { Id = UserId(Guid.NewGuid())
                  FirstName = firstName
                  LastName = lastName
                  Email = validEmail
                  UserType = userType
                  CreatedAt = DateTimeOffset.Now
                  UpdatedAt = DateTimeOffset.Now }
        | Error emailError -> Error emailError

    let createDriver firstName lastName email licenseNumber =
        let driverInfo =
            { LicenseNumber = licenseNumber
              VehicleInfo = None }

        createUser firstName lastName email (Driver driverInfo)

    let createRider firstName lastName email =
        createUser firstName lastName email Rider

    let displayName user =
        sprintf "%s %c." user.FirstName user.LastName.[0]

    let isDriver user =
        match user.UserType with
        | Driver _ -> true
        | Rider -> false
