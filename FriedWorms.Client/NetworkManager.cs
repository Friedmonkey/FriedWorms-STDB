using SpacetimeDB;
using SpacetimeDB.Types;

namespace FriedWorms.Client;

public delegate void DbEvent<T>(EventContext context, T row);
public class NetworkManager
{
    public event Action? OnConnected;
    public event Action? OnSubscriptionApplied;
    public DbEvent<Entity>? OnEntityInsert;
    public DbEvent<Explosion>? OnExplosionInsert;


    public DbConnection Conn { get; private set; }
    public Identity LocalIdentity { get; private set; }

    private string? AuthToken = null;
    private bool connected = false;
    public bool Subscribed { get; private set; }

    public void Start(string server)
    {
        var builder = DbConnection.Builder()
            .OnConnect(HandleConnect)
            .OnConnectError(HandleConnectError)
            .OnDisconnect(HandleDisconnect)
            .WithUri(server)
            .WithModuleName("worms");

        if (AuthToken != null)
        {
            builder = builder.WithToken(AuthToken);
        }

        Conn = builder.Build();
    }

    void HandleConnect(DbConnection _conn, Identity identity, string token)
    {
        Console.WriteLine("Connected!");
        AuthToken = token;
        LocalIdentity = identity;

        OnConnected?.Invoke();
        if (OnEntityInsert != null)
            _conn.Db.Entities.OnInsert += OnEntityInsert.Invoke;

        if (OnExplosionInsert != null)
            _conn.Db.Explosions.OnInsert += OnExplosionInsert.Invoke;


        Conn.SubscriptionBuilder()
            .OnApplied(HandleSubscriptionApplied)
            .SubscribeToAllTables();

        connected = true;
    }

    void HandleConnectError(Exception e)
    {
        Console.WriteLine("Connection error:" + e.Message);
    }
    void HandleDisconnect(DbConnection conn, Exception? e)
    {
        Console.WriteLine($"Disconnected.");
        if (e != null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message);
            Console.ResetColor();
        }
    }

    void HandleSubscriptionApplied(SubscriptionEventContext context)
    {
        Console.WriteLine("Subscription applied!");
        OnSubscriptionApplied?.Invoke();
        Subscribed = true;
    }

    public bool IsConnected => (Conn?.IsActive ?? false) && connected;
    public void Disconnect()
    {
        Conn.Disconnect();
        Conn = null;
        connected = false;
        Subscribed = false;
    }
}
