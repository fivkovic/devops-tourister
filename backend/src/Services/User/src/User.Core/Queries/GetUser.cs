using FluentResults;
using Mediator;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using User.Core.Database;
using User.Core.Model;

namespace User.Core.Queries;

public static class GetUser
{
    public record Query(Guid UserId) : IQuery<Result<UserProfile>>;

    public class UserNotFound() : Error("User could not be found");

    public class Handler(UserContext context, ILogger<Handler> logger) : IQueryHandler<Query, Result<UserProfile>>
    {
        public async ValueTask<Result<UserProfile>> Handle(Query query, CancellationToken cancellationToken)
        {
            var user = await context.UserProfiles
                .Find(u => u.Id == query.UserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is not null) return user;

            logger.LogWarning("User {UserId} not found", query.UserId);

            return new UserNotFound();
        }
    }
}