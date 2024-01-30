using Mediator;
using Property.Core.Commands;
using Property.Core.Queries;
using Property.Core.Services;
using Shared.Security;
using System.Security.Claims;

namespace Property.API;

using Property.Core.Model;

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

        routes.MapPost("/properties/{id:guid}/settings", UpdateSettings)
              .RequireAuthorization(AuthorizedRoles.Host)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status200OK);

        routes.MapPost("/properties/{id:guid}/availability", CreateAvailability)
              .RequireAuthorization(AuthorizedRoles.Host)
              .ProducesValidationProblem()
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .Produces<Slot>();

        routes.MapGet("/properties/{id:guid}/availability", GetAvailability)
              .RequireAuthorization(AuthorizedRoles.Host)
              .Produces<Slot[]>();

        routes.MapDelete("/properties/{id:guid}/availability/{slotId:guid}", DeleteAvailability)
              .RequireAuthorization(AuthorizedRoles.Host)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status200OK);

        routes.MapGet("/properties/search", SearchProperties)
              .ProducesValidationProblem()
              .Produces<SearchProperties.Result>();

        routes.MapPost("/properties/{id:guid}/reservations", RequestReservation)
              .RequireAuthorization(AuthorizedRoles.Customer)
              .ProducesValidationProblem()
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .Produces<Reservation>();
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

    private static async Task<IResult> UpdateSettings(
        Guid id,
        [AsParameters] UpdateSettings.Command command,
        ClaimsPrincipal user,
        IMediator mediator
    )
    {
        command.ByUser(user.Id());

        var validation = command.Validate();
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToDictionary());
        }

        var result = await mediator.Send(command);
        if (result.IsSuccess) return Results.Ok();

        var error = result.Errors.First();
        return Results.Problem(error.Message, statusCode: StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> CreateAvailability(
        Guid id,
        CreateAvailability.Command command,
        ClaimsPrincipal user,
        IMediator mediator)
    {
        command.ByUser(user.Id());
        command.WithPropertyId(id);

        var validation = command.Validate();
        if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

        var result = await mediator.Send(command);
        if (result.IsSuccess) return Results.Ok(result.Value);

        var error = result.Errors.First();
        return Results.Problem(error.Message, statusCode: StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetAvailability(
        Guid id,
        [AsParameters] GetSlots.Query query,
        ClaimsPrincipal user,
        IMediator mediator
    )
    {
        query.ByUser(user.Id());
        query.WithPropertyId(id);

        var result = await mediator.Send(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteAvailability(
        Guid id,
        Guid slotId,
        ClaimsPrincipal user,
        IMediator mediator
    )
    {
        var command = new DeleteAvailability.Command(id, slotId);
        command.ByUser(user.Id());

        var result = await mediator.Send(command);
        if (result.IsSuccess) return Results.Ok();

        var error = result.Errors.First();
        return Results.Problem(error.Message, statusCode: StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> SearchProperties(
        [AsParameters] SearchProperties.Query query,
        IMediator mediator
    )
    {
        var validation = query.Validate();
        if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

        var result = await mediator.Send(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> RequestReservation(
        Guid id,
        RequestReservation.Command command,
        ClaimsPrincipal user,
        IMediator mediator
    )
    {
        command.ByUser(user.Id());
        command.WithPropertyId(id);

        var validation = command.Validate();
        if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

        var result = await mediator.Send(command);
        if (result.IsSuccess) return Results.Ok(result.Value);

        var error = result.Errors.First();
        return Results.Problem(error.Message, statusCode: StatusCodes.Status400BadRequest);
    }
}
