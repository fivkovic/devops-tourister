using MongoDB.Driver;

namespace Reservation.Core.Database;

using Reservation.Core.Model;

public class ReservationContext
{
    public IMongoClient Client { get; }
    public IMongoCollection<Reservation> Reservations { get; }
    public IMongoCollection<User> Users { get; }

    public ReservationContext(IMongoClient client, string databaseName)
    {
        Client = client;
        var database = client.GetDatabase(databaseName);
        Reservations = database.GetCollection<Reservation>(nameof(Reservation));
        Users = database.GetCollection<User>(nameof(User));
    }
}
