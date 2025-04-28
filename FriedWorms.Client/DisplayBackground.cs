using Raylib_cs;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    public class Background
    {
        public Background(int width, int height) 
        {
            this.width = width;
            this.height = height;
            Map = new ushort[width * height];
            renderTexture = LoadRenderTexture(width, height);
        }
        public RenderTexture2D renderTexture;
        public int width;
        public int height;
        public ushort[] Map;
        public List<BackgroundDecorator> Decorators;
        public List<Color> colorMap = new List<Color>() { new Color(0,0,0,0) };
        public void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return;

            int pixelIndex = y * width + x;

            Color blendedColor = color;

            if (color.A < 255) //if we wanna write transparent pixel
            {
                ushort existingColorIndex = skyBackground.Map[pixelIndex];
                if (existingColorIndex != 0) //its not transparent we have to blend
                { 
                    Color existingColor = colorMap[existingColorIndex];
                    blendedColor = BlendColors(existingColor, color);
                }
            }

            int idx = colorMap.IndexOf(blendedColor);
            if (idx == -1)
            {
                idx = colorMap.Count;
                colorMap.Add(blendedColor);
            }
            skyBackground.Map[pixelIndex] = (ushort)idx;
        }

        // Simple average blending
        private Color BlendColors(Color a, Color b)
        {
            byte r = (byte)((a.R + b.R) / 2);
            byte g = (byte)((a.G + b.G) / 2);
            byte b2 = (byte)((a.B + b.B) / 2);
            byte a2 = (byte)((a.A + b.A) / 2); // Optional: blend alpha too

            return new Color(a2, r, g, b2);
        }
        public void Render()
        {
            BeginTextureMode(renderTexture);
            ClearBackground(new Color(0, 0, 0, 0));
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int index = y * width + x;

                    var color = colorMap[this.Map[index]];
                    DrawPixel(x, y, color);
                }
            }
            EndTextureMode();
        }
        public void Draw()
        {
            //scale
            var displayX = TARGET_WIDTH * BackgroundScale;
            var displayY = TARGET_HEIGHT * BackgroundScale;


            //offsets
            float mapX = CameraPosX;
            float mapY = CameraPosY;

            var src = new Rectangle(mapX, mapY, new Vector2(displayX, displayY));
            var dest = new Rectangle(0, 0, new Vector2(displayX, displayY));
            DrawTexturePro(renderTexture.Texture, src, dest, new(0, 0), 0, Color.White);
        }
    }
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

        var width = MapWidth * BackgroundScale;
        var height = MapHeight * BackgroundScale;
        skyBackground = new Background(width, height);

        for (int y = 0; y < height; y++)
        {
            float t = (float)y / (height - 1); // How far from top to bottom (0.0 to 1.0)
            Color rowColor = LerpColor(skyBottom, skyTop, t);

            for (int x = 0; x < width; x++)
            {
                skyBackground.SetPixel(x, y, rowColor);
            }
        }

        float[] stars = GenerateLayer(width, 0.01f);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width-1 || y == 0 || y == height-1)
                { 
                    skyBackground.SetPixel(x, y, Color.Red);
                    continue;
                }
                if (x % 20 == 1 || y % 20 == 1)
                { 
                    skyBackground.SetPixel(x, y, Color.Gray);
                    continue;
                }
                if (y <= stars[x] * height)
                { 
                    if (Random.Shared.Next(500) == 1)
                    { 
                        skyBackground.SetPixel(x, y, Color.White);
                    }
                }
            }
        }

        skyBackground.Render();
    }
    static void DisplayBackground()
    {
        skyBackground.Draw();
    }
    //static void DisplayBackground()
    //{
    //    DrawBackgroundFast(skyBackground);
    //}
    //static void DrawBackground(Background bg)
    //{
    //    if (bg is null) return;
    //    // how many world-pixels tall/wide we need to fill


    //    //// 2) decorators (clouds, planets…) with parallax
    //    //foreach (var d in bg.decorators)
    //    //{
    //    //    float sx = (d.WorldPos.X - CameraPosX) * BackgroundScale * d.Parallax;
    //    //    float sy = (d.WorldPos.Y - CameraPosY) * BackgroundScale * d.Parallax;
    //    //    DrawTexture(d.Tex, (int)sx, (int)sy, Color.White);
    //    //}
    //}
    //static void DrawBackgroundFast(Background bg)
    //{
    //    var displayX = TARGET_WIDTH * BackgroundScale;
    //    var displayY = TARGET_HEIGHT * BackgroundScale;

    //    // 1) gradient by scanlines
    //    for (int y = 0; y < displayY; y++)
    //    {
    //        float t = y / (float)(displayY - 1);
    //        Color c = LerpColor(bg.topColor, bg.bottomColor, t);
    //        // draw one horizontal line instead of thousands of pixels

    //        int offsetY = ((TARGET_WIDTH) / 2) + (int)(y - (CameraPosY));

    //        DrawLine(0, offsetY, displayX, offsetY, c);
    //    }

    //    //// 2) decorators as before
    //    //foreach (var d in bg.decorators)
    //    //{
    //    //    float sx = (d.WorldPos.X - CameraPosX) * BackgroundScale * d.Parallax;
    //    //    float sy = (d.WorldPos.Y - CameraPosY) * BackgroundScale * d.Parallax;
    //    //    DrawTexture(d.Tex, (int)sx, (int)sy, Color.White);
    //    //}
    //}

    static Color LerpColor(Color a, Color b, float t)
    {
        return new Color(
            (byte)(a.R + (b.R - a.R) * t),
            (byte)(a.G + (b.G - a.G) * t),
            (byte)(a.B + (b.B - a.B) * t),
            (byte)(a.A + (b.A - a.A) * t)
        );
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
