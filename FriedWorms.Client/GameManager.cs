using SpacetimeDB;
using SpacetimeDB.Types;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace FriedWorms.Client;

public class GameManager
{
    public event Action? OnConnected;
    public event Action? OnSubscriptionApplied;


    public DbConnection Conn { get; private set; }
    public Identity LocalIdentity { get; private set; }

    private string? AuthToken = null;
    private bool connected = false;

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
    }

    public bool IsConnected => (Conn?.IsActive ?? false) && connected;
    public void Disconnect()
    {
        Conn.Disconnect();
        Conn = null;
        connected = false;
    }
}
