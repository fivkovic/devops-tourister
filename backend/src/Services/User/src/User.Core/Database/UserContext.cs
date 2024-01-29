using MongoDB.Driver;
using User.Core.Model;

namespace User.Core.Database;

public class UserContext
{
    public IMongoClient Client { get; }
    public IMongoDatabase Database { get; }
    public IMongoCollection<UserProfile> UserProfiles { get; }

    public UserContext(IMongoClient client, string database)
    {
        Client = client;
        Database = client.GetDatabase(database);
        UserProfiles = Database.GetCollection<UserProfile>(nameof(UserProfiles));
    }
}