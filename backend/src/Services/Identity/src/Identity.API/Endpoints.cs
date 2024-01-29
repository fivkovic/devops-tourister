using Identity.Core.Commands;
using Mediator;
using System.Security.Claims;

namespace Identity.API;

public sealed class Endpoints
{
    private const string _prefix = "authentication";

    public static void Map(IEndpointRouteBuilder routes)
    {
        routes.MapPost(_prefix + "/signup", SignUp)
              .Produces(StatusCodes.Status200OK)
              .ProducesValidationProblem()
              .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Signs up a new user using the provided email and password.
    /// </summary>
    /// <remarks>Available roles are: "Host" and "Customer".</remarks>
    /// <response code="200">Returns the status code 200 when signing up the new user is successful</response>
    /// <response code="400">
    /// Returns a <see cref="ValidationProblemDetails"/> instance when the request fields are invalid <br></br>
    /// </response>
    private static async Task<IResult> SignUp(
        SignUpRequest request,
        IMediator mediator,
        ClaimsPrincipal principal,
        ILogger<Endpoints> logger,
        CancellationToken cancellationToken)
    {
        var validation = request.Validate();
        if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

        logger.LogInformation("Handling sign up request for email: {Email}", request.Email);

        var response = await mediator.Send(request, cancellationToken);
        if (response.IsSuccess)
        {
            logger.LogInformation("Successfully signed up new user with email: {Email}", request.Email);
            return Results.Ok();
        }

        var error = response.Errors.First().Message;
        logger.LogWarning("Sign up request failed for email: {Email}. Error={Error}", request.Email, error);

        return Results.Problem(error, "sign-up-failed", StatusCodes.Status400BadRequest);
    }
}
