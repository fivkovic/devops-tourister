using Property.Core.Model;

namespace Property.Core.Events;

public sealed record UserRegistered(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string Role
);

public sealed record UserUpdated(
    Guid UserId,
    string FirstName,
    string LastName,
    string Role
);

public sealed record ReservationRequested(
    Reservation Reservation
);

public sealed record ReservationCancelled(
    Guid ReservationId
);
