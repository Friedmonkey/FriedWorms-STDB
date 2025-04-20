using Raylib_cs;
using static Raylib_cs.Raylib;
using SpacetimeDB;
using SpacetimeDB.Types;
using SpacetimeDB.ClientApi;
using FriedWorms.Client;


namespace FriedWorms;

internal class Program
{
    static void Main(string[] args)
    {
        GameManager gameManager = new GameManager();
        //gameManager.OnConnected += MainGame;

        Console.WriteLine("Enter the server url:");
        string serverUrl = Console.ReadLine() ?? string.Empty;
        gameManager.Start(serverUrl);

        Console.ReadLine();
        //MainGame();
    }
    static void MainGame()
    {
        InitWindow(800, 480, "Hello World");

        while (!WindowShouldClose())
        {
            BeginDrawing();
            ClearBackground(Color.White);

            DrawText("Hello, world!", 12, 12, 20, Color.Black);

            EndDrawing();
        }

        CloseWindow();
    }
}
