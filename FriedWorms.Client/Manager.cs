using Raylib_cs;
using SpacetimeDB.Types;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    static byte[] Map = new byte[0];
    static int MapWidth = 0;
    static int MapHeight = 0;

    static float CameraPosX = 0.0f;
    static float CameraPosY = 0.0f;

    static float Zoom = 1.0f;

    static void Load(Config config)
    {
        MapWidth = config.MapWidth;
        MapHeight = config.MapHeight;
        Map = new byte[MapWidth * MapHeight];
        CreateMap();
    }
    static void Tick()
    { 
        if (IsKeyPressed(KeyboardKey.M))
            CreateMap();

        float elapsedTime = GetFrameTime();

        if (IsKeyDown(KeyboardKey.Equal)) Zoom += 0.1f;
        if (IsKeyDown(KeyboardKey.Minus)) Zoom -= 0.1f;
        Zoom = Math.Clamp(Zoom, 1.0f, 5.0f);


        float mapScrollSpeed = 400.0f / Zoom;

        if (IsKeyDown(KeyboardKey.Up))
            CameraPosY -= mapScrollSpeed * elapsedTime;
        if (IsKeyDown(KeyboardKey.Down))
            CameraPosY += mapScrollSpeed * elapsedTime;

        if (IsKeyDown(KeyboardKey.Left))
            CameraPosX -= mapScrollSpeed * elapsedTime;
        if (IsKeyDown(KeyboardKey.Right))
            CameraPosX += mapScrollSpeed * elapsedTime;

        float viewWidth = TARGET_WIDTH / Zoom;
        float viewHeight = TARGET_HEIGHT / Zoom;

        if (CameraPosX < 0) CameraPosX = 0;
        if (CameraPosX >= MapWidth - viewWidth) CameraPosX = MapWidth - viewWidth;
        if (CameraPosY < 0) CameraPosY = 0;
        if (CameraPosY >= MapHeight - viewHeight) CameraPosY = MapHeight - viewHeight;

    }

    static void CreateMap()
    {
        float[] Surface = new float[MapWidth];
        float[] NoiseSeed = new float[MapWidth];

        for (int i = 0; i < MapWidth; i++)
            NoiseSeed[i] = Random.Shared.NextSingle();

        NoiseSeed[0] = 0.5f;
        PerlinNoise1D(MapWidth, NoiseSeed, 8, 2.0f, ref Surface);

        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                if (y >= Surface[x] * MapHeight)
                    Map[y * MapWidth + x] = 1;
                else
                    Map[y * MapWidth + x] = 0;
            }
        }
    }

    // Taken from Perlin Noise Video https://youtu.be/6-0UaeJBumA
    static void PerlinNoise1D(int nCount, float[] fSeed, int nOctaves, float fBias, ref float[] fOutput)
    {
        // Used 1D Perlin Noise
        for (int x = 0; x < nCount; x++)
        {
            float fNoise = 0.0f;
            float fScaleAcc = 0.0f;
            float fScale = 1.0f;

            for (int o = 0; o < nOctaves; o++)
            {
                int nPitch = nCount >> o;
                int nSample1 = (x / nPitch) * nPitch;
                int nSample2 = (nSample1 + nPitch) % nCount;
                float fBlend = (float)(x - nSample1) / (float)nPitch;
                float fSample = (1.0f - fBlend) * fSeed[nSample1] + fBlend * fSeed[nSample2];
                fScaleAcc += fScale;
                fNoise += fSample * fScale;
                fScale = fScale / fBias;
            }

            // Scale to seed range
            fOutput[x] = fNoise / fScaleAcc;
        }
    }
}
