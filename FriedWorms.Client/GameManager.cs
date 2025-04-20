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

    //public static GameManager Instance => _instance ?? throw new Exception("Call the constructor first to load this");
    //private static GameManager? _instance;

    private string? AuthToken = null;


    //public void Start(string server)
    //{
    //    var builder = DbConnection.Builder()
    //        .OnConnect(HandleConnect)
    //        .OnConnectError(HandleConnectError)
    //        .OnDisconnect(HandleDisconnect)
    //        .WithUri(server)
    //        .WithModuleName("worms");

    //    if (AuthToken != null)
    //    {
    //        builder = builder.WithToken(AuthToken);
    //    }

    //    Conn = builder.Build();
    //}

    //void HandleConnect(DbConnection _conn, Identity identity, string token)
    //{
    //    Console.WriteLine("Connected!");
    //    AuthToken = token;
    //    LocalIdentity = identity;

    //    OnConnected?.Invoke();

    //    Conn.SubscriptionBuilder()
    //        .OnApplied(HandleSubscriptionApplied)
    //        .SubscribeToAllTables();
    //}
    //void HandleConnectError(Exception e)
    //{
    //    Console.WriteLine("Connection error:" + e.Message);
    //}
    //void HandleDisconnect(DbConnection conn, Exception? e)
    //{
    //    Console.WriteLine($"Disconnected.");
    //    if (e != null)
    //    { 
    //        Console.ForegroundColor = ConsoleColor.Red;
    //        Console.WriteLine(e.Message);
    //        Console.ResetColor();
    //    }
    //}

    //void HandleSubscriptionApplied(SubscriptionEventContext context)
    //{
    //    Console.WriteLine("Subscription applied!");
    //    OnSubscriptionApplied?.Invoke();
    //}

    public void Start(string server)
    {
        // In order to build a connection to SpacetimeDB we need to register
        // our callbacks and specify a SpacetimeDB server URI and module name.
        var builder = DbConnection.Builder()
            .OnConnect(HandleConnect)
            .OnConnectError(HandleConnectError)
            .OnDisconnect(HandleDisconnect)
            .WithUri(server)
            .WithToken("")
            .WithModuleName("worms");

        //// If the user has a SpacetimeDB auth token stored in the Unity PlayerPrefs,
        //// we can use it to authenticate the connection.
        //if (AuthToken != "")
        //{
        //    builder = builder.WithToken(AuthToken);
        //}

        // Building the connection will establish a connection to the SpacetimeDB
        // server.
        Conn = builder.Build();
    }

    // Called when we connect to SpacetimeDB and receive our client identity
    void HandleConnect(DbConnection _conn, Identity identity, string token)
    {
        Console.WriteLine("Connected.");
        AuthToken = token;
        LocalIdentity = identity;

        OnConnected?.Invoke();

        // Request all tables
        Conn.SubscriptionBuilder()
            .OnApplied(HandleSubscriptionApplied)
            .SubscribeToAllTables();
    }

    void HandleConnectError(Exception ex)
    {
        Console.WriteLine($"Connection error: {ex}");
    }

    void HandleDisconnect(DbConnection _conn, Exception? ex)
    {
        Console.WriteLine("Disconnected.");
        if (ex != null)
        {
            Console.WriteLine(ex);
        }
    }

    private void HandleSubscriptionApplied(SubscriptionEventContext ctx)
    {
        Console.WriteLine("Subscription applied!");
        OnSubscriptionApplied?.Invoke();
    }


    public bool IsConnected => Conn?.IsActive ?? false;
    public void Disconnect()
    {
        Conn.Disconnect();
        Conn = null;
    }
}
