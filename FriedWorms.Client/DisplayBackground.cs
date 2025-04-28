using Raylib_cs;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    public record Background(Color topColor, Color bottomColor, List<BackgroundDecorator> decorators);
    static Background skyBackground;

    static void LoadBackgrounds()
    {
        // define sky colours however you like:
        Color skyBottom = new Color(90, 200, 255, 255);  // light blue
        Color skyTop = new Color(15, 35, 85, 255);  // deep blue

        // build a decorator list once, maybe on Load():
        var decorators = new List<BackgroundDecorator>{
             new BackgroundDecorator(cloudTexure, new Vector2(100,50), 0.3f),
             //new BackgroundDecorator(planetTex,new Vector2(300,30), 0.1f),
        };
        skyBackground = new Background(skyTop, skyBottom, decorators);
    }
    static void DisplayBackground()
    {
        DrawBackgroundFast(skyBackground);
    }
    static void DrawBackground(Background bg)
    {
        if (bg is null) return;
        // how many world-pixels tall/wide we need to fill
        var displayX = TARGET_WIDTH / Zoom;
        var displayY = TARGET_HEIGHT / Zoom;

        // 1) gradient fill
        for (int x = 0; x < displayX; x++)
        {
            int mapX = x + (int)CameraPosX;
            if (mapX >= MapWidth) continue;

            for (int y = 0; y < displayY; y++)
            {
                int mapY = y + (int)CameraPosY;
                if (mapY >= MapHeight) continue;

                // t = 0 at top, 1 at bottom
                float t = y / (displayY - 1);
                Color c = LerpColor(bg.topColor, bg.bottomColor, t);
                DrawPixel(x, y, c);
            }
        }

        //// 2) decorators (clouds, planets…) with parallax
        //foreach (var d in bg.decorators)
        //{
        //    float sx = (d.WorldPos.X - CameraPosX) * BackgroundScale * d.Parallax;
        //    float sy = (d.WorldPos.Y - CameraPosY) * BackgroundScale * d.Parallax;
        //    DrawTexture(d.Tex, (int)sx, (int)sy, Color.White);
        //}
    }
    static void DrawBackgroundFast(Background bg)
    {
        var displayX = TARGET_WIDTH * BackgroundScale;
        var displayY = TARGET_HEIGHT * BackgroundScale;

        // 1) gradient by scanlines
        for (int y = 0; y < displayY; y++)
        {
            float t = y / (float)(displayY - 1);
            Color c = LerpColor(bg.topColor, bg.bottomColor, t);
            // draw one horizontal line instead of thousands of pixels

            int offsetY = ((TARGET_WIDTH) / 2) + (int)(y - (CameraPosY));

            DrawLine(0, offsetY, displayX, offsetY, c);
        }

        //// 2) decorators as before
        //foreach (var d in bg.decorators)
        //{
        //    float sx = (d.WorldPos.X - CameraPosX) * BackgroundScale * d.Parallax;
        //    float sy = (d.WorldPos.Y - CameraPosY) * BackgroundScale * d.Parallax;
        //    DrawTexture(d.Tex, (int)sx, (int)sy, Color.White);
        //}
    }

    static Color LerpColor(Color a, Color b, float t)
    {
        byte r = (byte)(a.R * (1 - t) + b.R * t);
        byte g = (byte)(a.G * (1 - t) + b.G * t);
        byte bl = (byte)(a.B * (1 - t) + b.B * t);
        return new Color(r, g, bl, (byte)255);
    }

    public struct BackgroundDecorator
    {
        public Texture2D Tex;     // your cloud/planet sprite
        public Vector2 WorldPos;  // world coords where it lives
        public float Parallax;    // e.g. 0.2f = moves 20% as fast as camera

        public BackgroundDecorator(Texture2D tex, Vector2 pos, float parallax)
        {
            Tex = tex;
            WorldPos = pos;
            Parallax = parallax;
        }
    }
}
