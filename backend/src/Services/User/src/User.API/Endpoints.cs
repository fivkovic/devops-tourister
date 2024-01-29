using Mediator;
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
    }

    private static async Task<IResult> Get(Guid id, ClaimsPrincipal user, IMediator mediator)
    {
        var query = new GetUser.Query(id);
        var result = await mediator.Send(query);

        if (result.IsSuccess) return Results.Ok(result.Value);

        var error = result.Errors.First();
        return Results.Problem(error.Message, statusCode: StatusCodes.Status400BadRequest);
    }
}