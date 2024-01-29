using FluentResults;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Property.Core.Database;

namespace Property.Core.Commands;

public static class DeleteAvailability
{
    public record Command(Guid PropertyId, Guid SlotId) : ICommand<Result>
    {
        public Guid UserId { get; private set; }

        public void ByUser(Guid userId) => UserId = userId;
    }

    public class AvailabilityNotFound() : Error("Availability period could not be found");

    public class Handler(PropertyContext context, ILogger<Handler> logger) : ICommandHandler<Command, Result>
    {
        public async ValueTask<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var deleted = await context.Slots
                .Where(s => s.Id == command.SlotId)
                .Where(s => s.Property.Id == command.PropertyId)
                .Where(s => s.Property.OwnerId == command.UserId)
                .ExecuteDeleteAsync(cancellationToken);

            if (deleted == 0) return new AvailabilityNotFound();

            logger.LogInformation("Deleted availability period {SlotId} for property {PropertyId}",
                                   command.SlotId, command.PropertyId);

            return Result.Ok();
        }
    }
}
