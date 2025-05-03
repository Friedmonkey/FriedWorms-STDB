using static Raylib_cs.Raylib;
using SpacetimeDB.Types;


namespace FriedWorms.Client;

partial class Program
{
    static NetworkManager gameManager;
    static void Main(string[] args)
    {
        gameManager = new NetworkManager();
        gameManager.OnEntityInsert += GameManager_OnEntityInsert;
        gameManager.OnEntityUpdate += GameManager_OnEntityUpdate;
        gameManager.OnEntityDelete += GameManager_OnEntityDelete;

        gameManager.OnExplosionInsert += GameManager_OnExplosionInsert;

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
            while (!ct.IsCancellationRequested && !(gameManager.IsConnected && gameManager.Subscribed))
            {
                conn.FrameTick();
                Thread.Sleep(100);
            }

            Console.WriteLine("Connection success enering fast ticking mode");

            while (!ct.IsCancellationRequested)
            {
                conn.FrameTick();
                Thread.Sleep(1);
            }
        }
        finally
        {
            conn.Disconnect();
        }
    }
}
