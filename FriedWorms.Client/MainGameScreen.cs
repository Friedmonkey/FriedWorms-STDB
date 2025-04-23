using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    //const int TARGET_WIDTH = 1280;
    //const int TARGET_HEIGHT = 720;
    //const int TARGET_WIDTH = 256;
    //const int TARGET_HEIGHT = 160;
    //const int TARGET_WIDTH = 320;
    //const int TARGET_HEIGHT = 200;
    const int TARGET_WIDTH = 384;
    const int TARGET_HEIGHT = 240;
    const int OverlayScale = 4;
    const int UiScale = 4;
    static void MainGame()
    {
        Load();

        SetConfigFlags(ConfigFlags.ResizableWindow);
        InitWindow(TARGET_WIDTH*2, TARGET_HEIGHT*2, "Hello World");
        SetTargetFPS(60);

        RenderTexture2D renderTexture = LoadRenderTexture(TARGET_WIDTH, TARGET_HEIGHT);
        RenderTexture2D overlayRenderTexture = LoadRenderTexture(TARGET_WIDTH*OverlayScale, TARGET_HEIGHT* OverlayScale);
        RenderTexture2D UIRenderTexture = LoadRenderTexture(TARGET_WIDTH * UiScale, TARGET_HEIGHT * UiScale);

        while (!WindowShouldClose())
        {
            int windowWidth = GetScreenWidth();
            int windowHeight = GetScreenHeight();

            Tick();

            BeginTextureMode(renderTexture);
            ClearBackground(Color.White);
            Display();
            EndTextureMode();

            BeginTextureMode(overlayRenderTexture);
            ClearBackground(new Color(0, 0, 0, 0));
            Overlay();
            EndTextureMode();

            BeginTextureMode(UIRenderTexture);
            ClearBackground(new Color(0, 0, 0, 0));
            RenderUI();
            EndTextureMode();

            BeginDrawing();
            ClearBackground(Color.RayWhite);
            
            DrawTextureScaled(renderTexture, TARGET_WIDTH, TARGET_HEIGHT);
            DrawTextureScaled(overlayRenderTexture, TARGET_WIDTH * OverlayScale, TARGET_HEIGHT * OverlayScale);
            DrawTextureScaled(UIRenderTexture, TARGET_WIDTH * UiScale, TARGET_HEIGHT * UiScale);
            EndDrawing();





            void DrawTextureScaled(RenderTexture2D texture, float width, float hight)
            {
                float scale = Math.Min(windowWidth / (float)width, windowHeight / (float)hight);

                //float zoom = scale * Zoom; // this replaces the previous scale logic
                float zoom = (windowWidth / (float)width) * Zoom;

                // Calculate new width and height of the scaled texture
                int scaledWidth = (int)(width * zoom);
                int scaledHeight = (int)(hight * zoom);

                // Offset so the zoom centers in the middle of the screen
                int offsetX = (windowWidth - scaledWidth) / 2;
                int offsetY = (windowHeight - scaledHeight) / 2;

                DrawTexturePro(
                    texture.Texture,
                    new Rectangle(0, 0, width, -hight), // Source (flipped Y)
                    new Rectangle(offsetX, offsetY, scaledWidth, scaledHeight), // Destination
                    new Vector2(0, 0), // Origin
                    0,
                    Color.White
                );
            }
        }

        UnloadRenderTexture(renderTexture);
    }
}
