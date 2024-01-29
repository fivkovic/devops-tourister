using Mediator;
using Property.Core.Commands;
using Property.Core.Services;
using Shared.Security;
using System.Security.Claims;

namespace Property.API;

using Property.Core.Model;
using Property.Core.Queries;

public static class Endpoints
{
    public static void Map(IEndpointRouteBuilder routes)
    {
        routes.MapPost("/properties", CreateProperty)
              .RequireAuthorization(AuthorizedRoles.Host)
              .ProducesValidationProblem()
              .Produces<Property>()
              .DisableAntiforgery();

        routes.MapGet("/properties", GetProperties)
              .RequireAuthorization(AuthorizedRoles.Host)
              .Produces<Property[]>();

        routes.MapGet("/properties/{id:guid}", GetProperty)
              .Produces(StatusCodes.Status404NotFound)
              .Produces<Property>();
    }

    private static async Task<IResult> CreateProperty(
        [AsParameters] CreateProperty.Command command,
        IFormFileCollection files,
        ClaimsPrincipal user,
        IMediator mediator,
        ImageService imageService)
    {
        command.ByUser(user.Id());

        var validation = command.Validate();
        if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

        var images = files
            .Take(10)
            .Where(f => f.ContentType.StartsWith("image/"))
            .Where(f => f.Length is > 0 and <= 1024 * 1024 * 5)
            .Select(f => (
                f.OpenReadStream(),
                f.ContentType.Split("/").ElementAtOrDefault(1)
            ));

        var imageIds = await imageService.SaveImages(images);
        command.WithImages(imageIds);

        var property = await mediator.Send(command);
        return Results.Ok(property);
    }

    private static async Task<IResult> GetProperties(
        ClaimsPrincipal user,
        Mediator mediator)
    {
        var command = new GetProperties.Query(user.Id());
        var result = await mediator.Send(command);
        return Results.Ok(result);
    }


    private static async Task<IResult> GetProperty(
        Guid id,
        IMediator mediator)
    {
        var query = new GetProperty.Query(id);
        var result = await mediator.Send(query);

        if (result.IsSuccess) return Results.Ok(result.Value);

        var error = result.Errors.First();
        return Results.Problem(error.Message, statusCode: StatusCodes.Status400BadRequest);
    }
}
