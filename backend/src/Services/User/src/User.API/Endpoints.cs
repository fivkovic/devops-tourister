using Mediator;
using Microsoft.AspNetCore.Authentication;
using Shared.Security;
using System.Security.Claims;
using User.Core.Commands;
using User.Core.Model;
using User.Core.Queries;

namespace User.API;

public static class Endpoints
{
    public static void MapEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/users/{id:guid}", Get)
              .RequireAuthorization()
              .Produces<UserProfile>()
              .ProducesProblem(StatusCodes.Status400BadRequest);

        routes.MapPut("/users/{id:guid}", Update)
              .RequireAuthorization()
              .ProducesValidationProblem()
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .Produces<UserProfile>();

        routes.MapDelete("/users", Delete)
              .RequireAuthorization()
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status200OK);

        routes.MapGet("/users/{id:guid}/reviews", GetReviews)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .Produces<List<Review>>();

        routes.MapDelete("/users/{id:guid}/reviews/{reviewId:guid}", DeleteReview)
              .RequireAuthorization(AuthorizedRoles.Customer)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status200OK);
    }

    private static async Task<IResult> Get(Guid id, ClaimsPrincipal user, IMediator mediator)
    {
        var query = new GetUser.Query(id);
        var result = await mediator.Send(query);

        if (result.IsSuccess) return Results.Ok(result.Value);

        var error = result.Errors.First();
        return Results.Problem(error.Message, statusCode: StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> Update(
        Guid id,
        UpdateUserProfile.Command command,
        ClaimsPrincipal user,
        IMediator mediator
    )
    {
        if (id != user.Id()) return Results.Forbid();

        command.ByUser(user.Id());

        var validation = command.Validate();
        if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

        var result = await mediator.Send(command);
        if (result.IsSuccess) return Results.Ok(result);

        var error = result.Errors.First();
        return Results.Problem(error.Message, statusCode: StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> Delete(ClaimsPrincipal user, HttpContext context, IMediator mediator)
    {
        var accessToken = await context.GetTokenAsync("access_token") ?? string.Empty;

        var command = new DeleteUser.Command(user.Id(), accessToken);
        var result = await mediator.Send(command);

        if (result.IsSuccess) return Results.Ok();

        var error = result.Errors.First();
        return Results.Problem(error.Message, statusCode: StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetReviews(Guid id, IMediator mediator)
    {
        var query = new GetReviews.Query(id);
        var result = await mediator.Send(query);

        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteReview(
        Guid id,
        Guid reviewId,
        ClaimsPrincipal user,
        IMediator mediator
    )
    {
        var command = new DeleteReview.Command(user.Id(), reviewId);
        var result = await mediator.Send(command);

        if (result.IsSuccess) return Results.Ok();

        var error = result.Errors.First();
        return Results.Problem(error.Message, statusCode: StatusCodes.Status400BadRequest);
    }

}