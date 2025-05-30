namespace Rideshare.Domain

open System
open ValueObjects
open Location
open User

/// Contains domain logic for managing trips in the Rideshare application.
module Trip =
    type TripId = TripId of Guid
    type TripRequestId = TripRequestId of Guid

    /// Represents the lifecycle of a trip, with explicit timestamps for each status
    type TripStatus =
        | Requested of requestedAt: DateTimeOffset
        | InProgress of startedAt: DateTimeOffset
        | Completed of completedAt: DateTimeOffset
        | Cancelled of cancelledAt: DateTimeOffset

    /// Represents a request to create a trip.
    type TripRequest =
        { Id: TripRequestId
          RiderId: UserId
          StartLocation: LocationId
          EndLocation: LocationId
          Status: TripStatus
          CreatedAt: DateTimeOffset
          UpdatedAt: DateTimeOffset }

    /// Represents a trip that has been accepted by a driver.
    type Trip =
        { Id: TripId
          TripRequestedId: TripRequestId
          DriverId: UserId
          Rating: Rating option
          Status: TripStatus
          CreatedAt: DateTimeOffset
          UpdatedAt: DateTimeOffset }

    /// Creates a new trip request for the given rider and locations.
    let createTripRequest riderId startLocationId endLocationId =
        { Id = TripRequestId(Guid.NewGuid())
          RiderId = riderId
          StartLocation = startLocationId
          EndLocation = endLocationId
          Status = Requested(DateTimeOffset.Now)
          CreatedAt = DateTimeOffset.Now
          UpdatedAt = DateTimeOffset.Now }

    let createTrip tripRequestId driverId =
        { Id = TripId(Guid.NewGuid())
          TripRequestedId = tripRequestId
          DriverId = driverId
          Rating = None
          Status = InProgress(DateTimeOffset.Now)
          CreatedAt = DateTimeOffset.Now
          UpdatedAt = DateTimeOffset.Now }

    // State transition Functions

    let startTrip trip =
        match trip.Status with
        | Requested _ ->
            Ok
                { trip with
                    Status = InProgress(DateTimeOffset.Now)
                    UpdatedAt = DateTimeOffset.Now }
        | _ -> Error(BusinessRuleViolation "Trip can only be started from Requested status")

    let completeTrip trip =
        match trip.Status with
        | InProgress _ ->
            Ok
                { trip with
                    Status = Completed(DateTimeOffset.Now)
                    UpdatedAt = DateTimeOffset.Now }
        | _ -> Error(BusinessRuleViolation "Trip can only be completed from InProgress status")


    let cancelTrip trip =
        match trip.Status with
        | Requested _ ->
            Ok
                { trip with
                    Status = Cancelled(DateTimeOffset.Now)
                    UpdatedAt = DateTimeOffset.Now }
        | _ -> Error(BusinessRuleViolation "Trip cannot be cancelled from current status")

    let isCompleted trip =
        match trip.Status with
        | Completed _ -> true
        | _ -> false

    let getCompletionDate trip =
        match trip.Status with
        | Completed completedAt -> Some completedAt
        | _ -> None

    let getTripDuration trip =
        match trip.Status with
        | Completed completedAt -> Some(completedAt - trip.CreatedAt)
        | _ -> None

    let getDriverId trip = trip.DriverId
    let getRiderId trip = trip.RiderId
    let getRating trip = trip.Rating |> Option.map Rating.value
    let getStats trip = trip.Status
