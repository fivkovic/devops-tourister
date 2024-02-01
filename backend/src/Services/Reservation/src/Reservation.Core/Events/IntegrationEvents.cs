namespace Tourister.Events;

using Reservation.Core.Model;

public record UserRegistered(
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

public record ReservationRejected(
    Guid ReservationId
);

public record ReservationCancelled(
    Guid ReservationId
);

public record HostReviewed(
    Guid CustomerId,
    Guid OwnerId,
    int Rating,
    string Content,
    DateTimeOffset Timestamp
);

public record PropertyReviewed(
    Guid ReservationId,
    int Rating,
    string Content,
    DateTimeOffset Timestamp
);