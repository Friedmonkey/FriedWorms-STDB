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

            int windowWidth = GetScreenWidth();
            int windowHeight = GetScreenHeight();

            float scale = Math.Min(windowWidth / (float)TARGET_WIDTH, windowHeight / (float)TARGET_HEIGHT);

            int scaledWidth = (int)(TARGET_WIDTH * scale);
            int scaledHeight = (int)(TARGET_HEIGHT * scale);

            int offsetX = (windowWidth - scaledWidth) / 2;
            int offsetY = (windowHeight - scaledHeight) / 2;
            
            BeginDrawing();
            ClearBackground(Color.RayWhite);
            DrawTexturePro(renderTexture.Texture,
                new Rectangle(0, 0, TARGET_WIDTH, -TARGET_HEIGHT), // Flip Y
                new Rectangle(offsetX, offsetY, scaledWidth, scaledHeight),
                new Vector2(0, 0),
                0,
                Color.White);
            EndDrawing();
        }

        UnloadRenderTexture(renderTexture);
    }
}
