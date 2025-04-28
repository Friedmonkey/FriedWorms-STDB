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
                    blendedColor = BlendStar2(existingColor, color, 2f);
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
        static byte Clamp(int value)
        {
            return (byte)(value < 0 ? 0 : (value > 255 ? 255 : value));
        }

        static Color BlendStar2(Color background, Color star, float curvePower = 0.5f)
        {
            float starAlpha = star.A / 255f;

            // remap brightness non-linearly
            float rRatio = background.R / 255f;
            float gRatio = background.G / 255f;
            float bRatio = background.B / 255f;

            float adjustedR = MathF.Pow(rRatio, curvePower);
            float adjustedG = MathF.Pow(gRatio, curvePower);
            float adjustedB = MathF.Pow(bRatio, curvePower);

            byte newR = (byte)(adjustedR * 255f);
            byte newG = (byte)(adjustedG * 255f);
            byte newB = (byte)(adjustedB * 255f);

            // Now blend the star onto the ADJUSTED background
            byte r = Clamp((int)(newR + star.R * (starAlpha*2)));
            byte g = Clamp((int)(newG + star.G * (starAlpha*2)));
            byte b = Clamp((int)(newB + star.B * (starAlpha*2)));

            return new Color(r, g, b, (byte)255);
        }

        public void Render()
        {
            Console.WriteLine($"Rendering texture with {skyBackground.colorMap.Count} colors!");
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
            DrawCircle((width / 2), (height / 2), 50, Color.Red);
            EndTextureMode();
        }
        public void Draw()
        {
            //scale
            var displayX = MathF.Round(TARGET_WIDTH * BackgroundScale);
            var displayY = MathF.Round(TARGET_HEIGHT * BackgroundScale);

            //offsets
            float mapX = MathF.Round(CameraPosX * BackgroundScale);
            float mapY = MathF.Round(CameraPosY * BackgroundScale);

            var src = new Rectangle(mapX, mapY, new Vector2(displayX, displayY));
            var dest = new Rectangle(0, 0, new Vector2(TARGET_WIDTH * BackgroundScale, TARGET_HEIGHT * BackgroundScale));
            DrawTexturePro(renderTexture.Texture, src, dest, new(0, 0), 0, Color.White);
        }
    }
    static Background skyBackground;

    static void LoadBackgrounds()
    {
        //rgb(139, 0, 41) - rgb(233, 208, 255)
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
            const float offset = 0.42f;
            const float scaling = 1.4f;
            float t = (float)y / (height - 1); // 0.0 -> 1.0
            t = t * scaling - offset;          // Apply your custom scaling
            t = Math.Clamp(t, 0.0f, 1.0f);

            t = t * t; // <--- ADD THIS! makes it start slow and end faster :)

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
                if (height-y <= stars[x] * height)
                { 
                    if (Random.Shared.Next(500+((height-y))*15) == 1)
                    { 
                        //draw a star
                        skyBackground.SetPixel(x, y, new Color(244, 255, 125, 125));

                        skyBackground.SetPixel(x, y+1, new Color(251, 255, 209, 75));
                        skyBackground.SetPixel(x, y-1, new Color(251, 255, 209, 75));

                        skyBackground.SetPixel(x+1, y, new Color(251, 255, 209, 75));
                        skyBackground.SetPixel(x-1, y, new Color(251, 255, 209, 75));
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
