namespace User.Core.Events;

public sealed record UserRegistered(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    string Residence
);

internal sealed record UserUpdated(
    Guid UserId,
    string FirstName,
    string LastName,
    string Role,
    string Residence
);

public sealed record HostReviewed(
    Guid CustomerId,
    Guid OwnerId,
    int Rating,
    string Content,
    DateTimeOffset Timestamp
);
