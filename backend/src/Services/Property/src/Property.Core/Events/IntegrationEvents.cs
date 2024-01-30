using Property.Core.Model;

namespace Tourister.Events;

public record UserRegistered(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string Role
);

public record UserUpdated(
    Guid UserId,
    string FirstName,
    string LastName,
    string Role
);

public record UserDeleted(
    Guid UserId
);

public record ReservationRequested(
    Reservation Reservation
);

public record ReservationAccepted(
    Guid ReservationId
);

public record ReservationCancelled(
    Guid ReservationId
);

public record PropertyReviewed(
    Guid ReservationId,
    int Rating,
    string Content,
    DateTimeOffset Timestamp
);
