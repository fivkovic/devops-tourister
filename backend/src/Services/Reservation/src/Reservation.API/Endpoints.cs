using Mediator;
using Reservation.Core.Commands;
using Reservation.Core.Queries;
using Shared.Security;
using System.Security.Claims;

namespace Reservation.API;

using Reservation.Core.Model;

public static class Endpoints
{
    public static void MapEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/reservations", Get)
              .RequireAuthorization()
              .ProducesValidationProblem()
              .Produces<Reservation[]>();

        routes.MapDelete("/reservations/{id:guid}", DeleteReservation)
              .RequireAuthorization(AuthorizedRoles.Customer)
              .ProducesValidationProblem()
              .Produces(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest);

        routes.MapPost("/reservations/{id:guid}/cancel", Cancel)
              .RequireAuthorization(AuthorizedRoles.Customer)
              .Produces<Reservation>()
              .Produces(StatusCodes.Status404NotFound)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .ProducesValidationProblem();

        routes.MapGet("/reservations/active", ActiveReservations)
              .RequireAuthorization()
              .Produces<bool>();

        routes.MapPost("/reservations/{id:guid}/update", UpdateReservation)
              .RequireAuthorization(AuthorizedRoles.Host)
              .Produces<Reservation>()
              .ProducesProblem(StatusCodes.Status404NotFound);

        routes.MapPost("/reservations/{id:guid}/review", AddReview)
              .RequireAuthorization(AuthorizedRoles.Customer)
              .Produces(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .ProducesValidationProblem();

        routes.MapGet("/reservations/notifications", GetNotifications)
              .RequireAuthorization()
              .Produces<List<Notification>>();

        routes.MapDelete("/reservations/notifications/{id:guid}", DeleteNotification)
              .RequireAuthorization()
              .Produces(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest);

        routes.MapGet("/reservations/notifications/settings", GetSubscriptionSettings)
              .RequireAuthorization()
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .Produces<Subscription>();

        routes.MapPost("/reservations/notifications/settings", UpdateSubscriptionSettings)
              .RequireAuthorization()
              .Produces(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .ProducesValidationProblem();
    }

    private static async Task<IResult> Get(
        int page,
        string sort,
        ReservationStatus? status,
        ClaimsPrincipal user,
        IMediator mediator
    )
    {
        var query = new GetReservations.Query(page, sort, status);
        query.ByUser(user.Id(), user.Role());

        var validation = query.Validate();
        if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

        var result = await mediator.Send(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteReservation(Guid id, ClaimsPrincipal user, IMediator mediator)
    {
        var command = new DeleteReservation.Command(id, user.Id());

        var result = await mediator.Send(command);
        if (result.IsSuccess) return Results.Ok();

        var error = result.Errors.First();
        return Results.Problem(error.Message, statusCode: StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> Cancel(Guid id, ClaimsPrincipal user, IMediator mediator)
    {
        var command = new CancelReservation.Command(id, user.Id());
        var result = await mediator.Send(command);

        if (result.IsSuccess) return Results.Ok(result.Value);

        var error = result.Errors.First();
        return error switch
        {
            CancelReservation.ReservationNotFound => Results.NotFound(),
            CancelReservation.ReservationUncancellable or _ => Results.Problem(
                error.Message,
                "RESERVATION_UNCANCELLABLE",
                StatusCodes.Status400BadRequest
            ),
        };
    }

    private static async Task<IResult> ActiveReservations(ClaimsPrincipal user, IMediator mediator)
    {
        var query = new HasActiveReservations.Query(user.Id(), user.Role());
        var result = await mediator.Send(query);

        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateReservation(
        Guid id,
        bool accepted,
        ClaimsPrincipal user,
        IMediator mediator
    )
    {
        var command = new UpdateReservation.Command(id, user.Id(), accepted);

        var result = await mediator.Send(command);
        if (result.IsSuccess) return Results.Ok(result.Value);

        var error = result.Errors.First();
        return Results.Problem(error.Message, statusCode: StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> AddReview(
        Guid id,
        [AsParameters] AddReview.Command command,
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

    private static async Task<IResult> GetNotifications(ClaimsPrincipal user, IMediator mediator)
    {
        var query = new GetNotifications.Query(user.Id());
        var result = await mediator.Send(query);

        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteNotification(
        Guid id,
        ClaimsPrincipal user,
        IMediator mediator
    )
    {
        var command = new DeleteNotification.Command(id, user.Id());
        var result = await mediator.Send(command);

        if (result.IsSuccess) return Results.Ok();

        var error = result.Errors.First();
        return Results.Problem(error.Message, statusCode: StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetSubscriptionSettings(ClaimsPrincipal user, IMediator mediator)
    {
        var query = new GetSubscriptionSettings.Query(user.Id());
        var result = await mediator.Send(query);

        if (result.IsSuccess) return Results.Ok(result.Value);

        var error = result.Errors.First();
        return Results.Problem(error.Message, statusCode: StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> UpdateSubscriptionSettings(
        [AsParameters] UpdateSubscriptionSettings.Command command,
        ClaimsPrincipal user,
        IMediator mediator
    )
    {
        command.ByUser(user.Id(), user.Role());

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
}
