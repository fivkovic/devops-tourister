using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Identity.Core.DTO;
using Identity.Core.Model;
using Identity.Core.Security;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Identity.Core.Commands;

public sealed record SignInRequest(string Email, string Password) : IRequest<Result<SignInResponse>>
{
    public class Validator : AbstractValidator<SignInRequest>
    {
        public static readonly Validator Instance = new();

        public Validator()
        {
            RuleFor(o => o.Email).NotEmpty().MaximumLength(200).EmailAddress();
            RuleFor(o => o.Password).NotEmpty().MaximumLength(200);
        }
    }

    public ValidationResult Validate() => Validator.Instance.Validate(this);
}

public sealed record SignInResponse(UserDto User, string Token);

public sealed class SignInError
{
    public const string EmailNotFound = "EmailNotFound";
    public const string IncorrectPassword = "IncorrectPassword";
}

public sealed class SignInHandler(UserManager<User> userManager, SignInManager<User> signInManager)
    : IRequestHandler<SignInRequest, Result<SignInResponse>>
{
    public async ValueTask<Result<SignInResponse>> Handle(SignInRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Result.Fail(SignInError.EmailNotFound);
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: false
        );

        if (!signInResult.Succeeded)
        {
            return Result.Fail(SignInError.IncorrectPassword);
        }

        return new SignInResponse(user.Adapt<UserDto>(), JwtProvider.GetAccessToken(user));
    }
}