using Raylib_cs;
using static Raylib_cs.Raylib;
using SpacetimeDB;
using SpacetimeDB.ClientApi;


namespace FriedWorms;

internal class Program
{
    static void Main(string[] args)
    {
        //SpacetimeDB.BSATN.Bool
        InitWindow(800, 480, "Hello World");

        while (!WindowShouldClose())
        {
            BeginDrawing();
            ClearBackground(Color.White);

            DrawText("Hello, world!", 12, 12, 20, Color.Black);

            EndDrawing();
        }

        CloseWindow();
        //Console.WriteLine("Hello, World!");
    }
}
