namespace Tourister.Events;

internal sealed record UserRegistered(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    string Residence
);

public sealed record UserUpdated(
    Guid UserId,
    string FirstName,
    string LastName,
    string Role,
    string Residence
);

public sealed record UserDeleted(
    Guid UserId
);
