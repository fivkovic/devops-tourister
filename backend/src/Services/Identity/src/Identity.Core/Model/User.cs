using Microsoft.AspNetCore.Identity;

namespace Identity.Core.Model;

public sealed class User : IdentityUser<Guid>
{
    public static class Roles
    {
        public const string Host = "Host";
        public const string Customer = "Customer";

        public static readonly List<string> All = [Host, Customer];
    }

    public string Role { get; set; } = null!;
    public string FirstName { get; set; }
    public string LastName { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public User() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public User(string email, string firstName, string lastName, string role)
    {
        Email = UserName = email;
        EmailConfirmed = true;
        FirstName = firstName;
        LastName = lastName;
        Role = role;
    }
}