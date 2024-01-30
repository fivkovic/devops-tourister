namespace Tourister.Events;

public record UserRegistered(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    string Residence
);

public record UserUpdated(
    Guid UserId,
    string FirstName,
    string LastName,
    string Role,
    string Residence
);

public record UserDeleted(
    Guid UserId
);

public record HostReviewed(
    Guid CustomerId,
    Guid OwnerId,
    int Rating,
    string Content,
    DateTimeOffset Timestamp
);
