using Raylib_cs;
using static Raylib_cs.Raylib;
using SpacetimeDB;
using SpacetimeDB.Types;
using SpacetimeDB.ClientApi;
using FriedWorms.Client;


namespace FriedWorms;

internal class Program
{
    static GameManager gameManager;
    static void Main(string[] args)
    {
        gameManager = new GameManager();
        //gameManager.OnConnected += MainGame;

        Console.WriteLine("Enter the server url:");
        string serverUrl = Console.ReadLine() ?? string.Empty;
        gameManager.Start(serverUrl);

        var cancellationTokenSource = new CancellationTokenSource();
        // Spawn a thread to call process updates and process commands
        var thread = new Thread(() => ProcessThread(gameManager.Conn, cancellationTokenSource.Token));
        thread.Start();
        // Handles the main game
        MainGame();
        // This signals the ProcessThread to stop
        cancellationTokenSource.Cancel();
        thread.Join();
        CloseWindow();
    }
    static void MainGame()
    {
        InitWindow(800, 480, "Hello World");

        while (!WindowShouldClose())
        {
            gameManager.Conn.FrameTick();
            BeginDrawing();
            ClearBackground(Color.White);

            DrawText("Hello, world!", 12, 12, 20, Color.Black);

            EndDrawing();
        }
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
