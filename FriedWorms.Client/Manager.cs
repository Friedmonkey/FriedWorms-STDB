using Raylib_cs;
using SpacetimeDB.Types;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    static List<Entity> Entities = new List<Entity>();
    static byte[] Map = new byte[0];
    static int MapWidth = 0;
    static int MapHeight = 0;

    static float CameraPosX = 0.0f;
    static float CameraPosY = 0.0f;

    static float Zoom = 2.0f;

    static void Load()
    {
        var config = gameManager.Conn.Db.Config.Id.Find(0);
        if (config is null)
            throw new Exception("Unable to get config from server!");

        MapWidth = config.MapWidth;
        MapHeight = config.MapHeight;

        CameraPosY = (MapHeight - TARGET_HEIGHT / Zoom) / 2;
        CameraPosX = (MapWidth - TARGET_WIDTH / Zoom) / 4;

        Map = new byte[MapWidth * MapHeight];
        CreateMap();

        Entities = gameManager.Conn.Db.Entities.Iter().ToList();
        if (Entities.Count == 0)
            Entities.Add(new Entity()
            {
                ModelData = (uint)EntityModelType.Dummy,
                Position = new(MapWidth / 2, MapHeight / 2)
            });
    }
    static void Tick()
    { 
        float elapsedTime = GetFrameTime();

        if (IsKeyPressed(KeyboardKey.M))
            CreateMap();

        if (IsMouseButtonPressed(MouseButton.Left) && TryGetMouseWorldPos(out var world))
        {
            Entities.Add(new Entity()
            {
                ModelData = (uint)EntityModelType.Missile,
                Position = new(world.X, world.Y),
            });
        }
        if (IsMouseButtonPressed(MouseButton.Right) && TryGetMouseWorldPos(out world))
        {
            Entities.Add(new Entity()
            {
                ModelData = (uint)EntityModelType.Dummy,
                Position = new(world.X, world.Y),
            });
        }

        if (IsMouseButtonPressed(MouseButton.Middle) && TryGetMouseWorldPos(out world))
        {
            Entities.Add(new Entity()
            {
                ModelData = (uint)EntityModelType.Worm,
                Position = new(world.X, world.Y),
            });
        }


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

    static bool TryGetMouseWorldPos(out Vector2 output)
    {
        output = Vector2.Zero;

        Vector2 mouse = GetMousePosition();

        int windowWidth = GetScreenWidth();
        int windowHeight = GetScreenHeight();

        float scale = Math.Min(windowWidth / (float)TARGET_WIDTH, windowHeight / (float)TARGET_HEIGHT);

        int scaledWidth = (int)(TARGET_WIDTH * scale);
        int scaledHeight = (int)(TARGET_HEIGHT * scale);

        int offsetX = (windowWidth - scaledWidth) / 2;
        int offsetY = (windowHeight - scaledHeight) / 2;

        // Check if mouse is inside the render texture part
        if (mouse.X < offsetX || mouse.X > offsetX + scaledWidth ||
            mouse.Y < offsetY || mouse.Y > offsetY + scaledHeight)
            return false; // Mouse is outside the game area :(

        // Convert to render texture space
        float renderMouseX = (mouse.X - offsetX) / scale;
        float renderMouseY = (mouse.Y - offsetY) / scale;

        // Convert to world space
        float worldX = renderMouseX / Zoom + CameraPosX;
        float worldY = renderMouseY / Zoom + CameraPosY;
        output = new Vector2(worldX, worldY);

        return true;
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
