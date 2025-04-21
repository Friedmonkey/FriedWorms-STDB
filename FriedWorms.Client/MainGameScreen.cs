using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    //const int TARGET_WIDTH = 1280;
    //const int TARGET_HEIGHT = 720;
    const int TARGET_WIDTH = 256*2;
    const int TARGET_HEIGHT = 160*2;
    static void MainGame()
    {
        Load();

        SetConfigFlags(ConfigFlags.ResizableWindow);
        InitWindow(TARGET_WIDTH*2, TARGET_HEIGHT*2, "Hello World");
        SetTargetFPS(60);

        RenderTexture2D renderTexture = LoadRenderTexture(TARGET_WIDTH, TARGET_HEIGHT);

        while (!WindowShouldClose())
        {
            Tick();
            BeginTextureMode(renderTexture);

            ClearBackground(Color.White);
            Display();
            EndTextureMode();

            // Use your custom Zoom value here
            int windowWidth = GetScreenWidth();
            int windowHeight = GetScreenHeight();
            float scale = Math.Min(windowWidth / (float)TARGET_WIDTH, windowHeight / (float)TARGET_HEIGHT);

            float zoom = scale * Zoom; // this replaces the previous scale logic
            // Calculate new width and height of the scaled texture
            int scaledWidth = (int)(TARGET_WIDTH * zoom);
            int scaledHeight = (int)(TARGET_HEIGHT * zoom);

            // Offset so the zoom centers in the middle of the screen
            int offsetX = (windowWidth - scaledWidth) / 2;
            int offsetY = (windowHeight - scaledHeight) / 2;
            BeginDrawing();
            ClearBackground(Color.RayWhite);


            // Draw the render texture scaled from center using your zoom
            DrawTexturePro(
                renderTexture.Texture,
                new Rectangle(0, 0, TARGET_WIDTH, -TARGET_HEIGHT), // Source (flipped Y)
                new Rectangle(offsetX, offsetY, scaledWidth, scaledHeight), // Destination
                new Vector2(0, 0), // Origin
                0,
                Color.White
            );

            EndDrawing();

        }

        UnloadRenderTexture(renderTexture);
    }
}
