namespace User.Core.Events;

public sealed record UserRegistered(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    string Residence
);
