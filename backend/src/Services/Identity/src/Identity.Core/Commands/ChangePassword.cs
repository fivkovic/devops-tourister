using FluentResults;
using FluentValidation;
using Identity.Core.Model;
using Mediator;
using Microsoft.AspNetCore.Identity;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Identity.Core.Commands;

public sealed record ChangePasswordRequest(string Password, string NewPassword) : IRequest<Result>
{
    public class Validator : AbstractValidator<ChangePasswordRequest>
    {
        public static readonly Validator Instance = new();

        public Validator()
        {
            RuleFor(o => o.Password).NotEmpty().MaximumLength(200);
            RuleFor(o => o.NewPassword).NotEmpty().MaximumLength(200);
        }
    }

    public Guid UserId { get; private set; }

    public void ByUser(Guid userId) => UserId = userId;
    public ValidationResult Validate() => Validator.Instance.Validate(this);
}

public class ChangePasswordError
{
    public const string UserNotFound = "UserNotFound";
    public const string IncorrectPassword = "IncorrectPassword";
}

public class ChangePasswordHandler(UserManager<User> userManager, SignInManager<User> signInManager)
    : IRequestHandler<ChangePasswordRequest, Result>
{
    public async ValueTask<Result> Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null) return Result.Fail(ChangePasswordError.UserNotFound);

        var result = await userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);
        if (result.Succeeded) return Result.Ok();

        var error = result.Errors.LastOrDefault()?.Description;
        return Result.Fail(error);
    }
}