using FluentResults;
using FluentValidation;
using Identity.Core.Database;
using Identity.Core.Model;
using MassTransit;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Tourister.Events;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Identity.Core.Commands;

public sealed record SignUpRequest(
    string Email,
    string Password,
    string Role,
    string FirstName,
    string LastName,
    string Residence) : IRequest<Result>
{
    public class Validator : AbstractValidator<SignUpRequest>
    {
        public static readonly Validator Instance = new();

        public Validator()
        {
            RuleFor(o => o.Email).NotEmpty().MaximumLength(100).EmailAddress();
            RuleFor(o => o.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(o => o.LastName).NotEmpty().MaximumLength(100);
            RuleFor(o => o.Residence).NotEmpty().MaximumLength(100);
            RuleFor(o => o.Role)
                .NotEmpty()
                .MaximumLength(10)
                .Must(User.Roles.All.Contains)
                .WithMessage($"Role must be one of: {string.Join(", ", User.Roles.All)}");
            RuleFor(o => o.Password).NotEmpty().MaximumLength(200);
        }
    }

    public ValidationResult Validate() => Validator.Instance.Validate(this);
}

public sealed class SignUpHandler(UserManager<User> userManager, IdentityContext context, IPublishEndpoint publisher)
    : IRequestHandler<SignUpRequest, Result>
{
    public async ValueTask<Result> Handle(SignUpRequest request, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var user = new User(request.Email, request.FirstName, request.LastName, request.Role);
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return Result.Fail(result.Errors.LastOrDefault()?.Description);

        var @event = new UserRegistered(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            user.Role,
            request.Residence
        );

        await publisher.Publish(@event, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Result.Ok();
    }
}