namespace Reservation.Core.Events;

using Reservation.Core.Model;

public sealed record UserRegistered(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string Role
);

public record ReservationRequested(
    Reservation Reservation
);

public record ReservationAccepted(
    Guid ReservationId
);