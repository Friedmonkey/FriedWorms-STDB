using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    //const int TARGET_WIDTH = 384;
    //const int TARGET_HEIGHT = 240;
    public const int TARGET_WIDTH = 448;
    public const int TARGET_HEIGHT = 280;
    const int OverlayScale = 4;
    const int UiScale = 4;
    public const int BackgroundScale = 4;
    static void MainGame()
    {
        SetConfigFlags(ConfigFlags.ResizableWindow);
        InitWindow(TARGET_WIDTH*2, TARGET_HEIGHT*2, "Hello World");
        SetTargetFPS(60);

        Load();

        RenderTexture2D backgroundTexture = LoadRenderTexture(TARGET_WIDTH * BackgroundScale, TARGET_HEIGHT * BackgroundScale);
        RenderTexture2D renderTexture = LoadRenderTexture(TARGET_WIDTH, TARGET_HEIGHT);
        RenderTexture2D overlayRenderTexture = LoadRenderTexture(TARGET_WIDTH*OverlayScale, TARGET_HEIGHT* OverlayScale);
        RenderTexture2D UIRenderTexture = LoadRenderTexture(TARGET_WIDTH * UiScale, TARGET_HEIGHT * UiScale);

        LoadBackgrounds();

        while (!WindowShouldClose())
        {
            int windowWidth = GetScreenWidth();
            int windowHeight = GetScreenHeight();

            Tick();

            BeginTextureMode(backgroundTexture);
            ClearBackground(new Color(0, 0, 0, 0));
            DisplayBackground();
            //spacegif.Draw(0,0, Color.White);
            //DrawTexture(spaceTexure, 0,0,Color.White);
            EndTextureMode();

            BeginTextureMode(renderTexture);
            ClearBackground(new Color(0, 0, 0, 0));
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
            
            DrawTextureScaled(backgroundTexture, TARGET_WIDTH, TARGET_HEIGHT);
            DrawTextureScaled(renderTexture, TARGET_WIDTH, TARGET_HEIGHT);
            DrawTextureScaled(overlayRenderTexture, TARGET_WIDTH * OverlayScale, TARGET_HEIGHT * OverlayScale);
            DrawTextureScaled(UIRenderTexture, TARGET_WIDTH * UiScale, TARGET_HEIGHT * UiScale, zoomAffected:false);
            EndDrawing();



            void DrawTextureScaled(RenderTexture2D texture, float width, float hight, bool zoomAffected = true)
            {
                float zoom = (windowWidth / (float)width);

                // Calculate new width and height of the scaled texture
                int scaledWidth = (int)(width * zoom);
                int scaledHeight = (int)(hight * zoom);

                // Offset so the zoom centers in the middle of the screen
                int offsetX = (windowWidth - scaledWidth) / 2;
                int offsetY = (windowHeight - scaledHeight) / 2;

                if (zoomAffected)
                { 
                    scaledWidth = (int)(scaledWidth * Zoom);
                    scaledHeight = (int)(scaledHeight * Zoom);
                }

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
        UnloadRenderTexture(overlayRenderTexture);
        UnloadRenderTexture(UIRenderTexture);
    }
}
