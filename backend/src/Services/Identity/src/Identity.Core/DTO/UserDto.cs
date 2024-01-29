namespace Identity.Core.DTO;

public sealed record UserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role
);