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
    static void MainGame()
    {
        Load();

        SetConfigFlags(ConfigFlags.ResizableWindow);
        InitWindow(TARGET_WIDTH*2, TARGET_HEIGHT*2, "Hello World");
        SetTargetFPS(60);

        RenderTexture2D renderTexture = LoadRenderTexture(TARGET_WIDTH, TARGET_HEIGHT);
        RenderTexture2D overlayRenderTexture = LoadRenderTexture(TARGET_WIDTH*OverlayScale, TARGET_HEIGHT* OverlayScale);

        while (!WindowShouldClose())
        {
            Tick();
            BeginTextureMode(renderTexture);

            ClearBackground(Color.White);
            Display();
            EndTextureMode();


            BeginTextureMode(overlayRenderTexture);
            ClearBackground(new Color(0,0,0,0));
            foreach (var entity in Entities.Where(e=>e.ModelData == (uint)EntityModelType.Dummy))
            {
                float angle = MathF.Atan2(entity.Velocity.Y, entity.Velocity.X);

                var elapsedTime = GetFrameTime();

                float potentialX = entity.Position.X + entity.Velocity.X * elapsedTime;
                float potentialY = entity.Position.Y + entity.Velocity.Y * elapsedTime;

                for (float radius = (angle - 3.14159f / 2.0f); radius < angle + 3.14159f / 2; radius += 3.14159f / 10.0f)
                {
                    float testPosX = (entity.Radius) * MathF.Cos(radius) + potentialX;
                    float testPosY = (entity.Radius) * MathF.Sin(radius) + potentialY;

                    // Constrain to test within map boundary
                    if (testPosX >= MapWidth) testPosX = MapWidth - 1;
                    if (testPosY >= MapHeight) testPosY = MapHeight - 1;
                    if (testPosX < 0) testPosX = 0;
                    if (testPosY < 0) testPosY = 0;

                    int newX = (int)Math.Round((testPosX * OverlayScale) - (CameraPosX * OverlayScale));
                    int newY = (int)Math.Round((testPosY * OverlayScale) - (CameraPosY * OverlayScale));

                    DrawPixel(newX, newY, Color.Red);
                }
            }
            EndTextureMode();

            // Use your custom Zoom value here
            int windowWidth = GetScreenWidth();
            int windowHeight = GetScreenHeight();
            float scale = Math.Min(windowWidth / (float)TARGET_WIDTH, windowHeight / (float)TARGET_HEIGHT);

            //float zoom = scale * Zoom; // this replaces the previous scale logic
            float zoom = (windowWidth / (float)TARGET_WIDTH) * Zoom;

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


            scale = Math.Min(windowWidth / (float)(TARGET_WIDTH*OverlayScale), windowHeight / (float)(TARGET_HEIGHT*OverlayScale));

            //float zoom = scale * Zoom; // this replaces the previous scale logic
            zoom = (windowWidth / (float)(TARGET_WIDTH * OverlayScale)) * Zoom;

            // Calculate new width and height of the scaled texture
            scaledWidth = (int)((TARGET_WIDTH * OverlayScale) * zoom);
            scaledHeight = (int)((TARGET_HEIGHT * OverlayScale) * zoom);

            // Offset so the zoom centers in the middle of the screen
            offsetX = (windowWidth - scaledWidth) / 2;
            offsetY = (windowHeight - scaledHeight) / 2;

            DrawTexturePro(
                overlayRenderTexture.Texture,
                new Rectangle(0, 0, (TARGET_WIDTH * OverlayScale), -(TARGET_HEIGHT * OverlayScale)), // Source (flipped Y)
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
