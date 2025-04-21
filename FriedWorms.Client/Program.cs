using Raylib_cs;
using static Raylib_cs.Raylib;
using SpacetimeDB;
using SpacetimeDB.Types;
using SpacetimeDB.ClientApi;
using FriedWorms.Client;


namespace FriedWorms.Client;

partial class Program
{
    static NetworkManager gameManager;
    static void Main(string[] args)
    {
        gameManager = new NetworkManager();
        //gameManager.OnConnected += MainGame;
        string serverUrl = string.Empty;

        if (File.Exists("../../../../server.txt"))
            serverUrl = File.ReadAllText("../../../../server.txt");
        else
        { 
            Console.WriteLine("Enter the server url:");
            serverUrl = Console.ReadLine() ?? string.Empty;
        }
        gameManager.Start(serverUrl);

        var cancellationTokenSource = new CancellationTokenSource();
        // Spawn a thread to call process updates and process commands
        var thread = new Thread(() => ProcessThread(gameManager.Conn, cancellationTokenSource.Token));
        thread.Start();

        Console.WriteLine("Connecting...");
        while (!(gameManager.IsConnected && gameManager.Subscribed))
        {
            Thread.Sleep(100);
        }
        // Handles the main game
        MainGame();
        // This signals the ProcessThread to stop
        cancellationTokenSource.Cancel();
        thread.Join();
        CloseWindow();
    }
    static void ProcessThread(DbConnection conn, CancellationToken ct)
    {
        try
        {
            // loop until cancellation token
            while (!ct.IsCancellationRequested)
            {
                conn.FrameTick();

                //ProcessCommands(conn.Reducers);

                Thread.Sleep(100);
            }
        }
        finally
        {
            conn.Disconnect();
        }
    }
}
