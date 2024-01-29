namespace Identity.Core.Events;

internal sealed record UserRegistered(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    string Residence
);