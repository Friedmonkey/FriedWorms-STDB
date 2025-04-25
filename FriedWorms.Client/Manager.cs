using Raylib_cs;
using SpacetimeDB.Types;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

public enum MapColor
{ 
    Skyblue = 0,
    Grass1 = 1,
    Grass2 = 2,
    Rock1 = 3,
    Rock2 = 4,

    Cloud = 5,


    Unknown = 255
}
partial class Program
{
    static List<Entity> Entities = new List<Entity>();
    static byte[] Map = new byte[0];
    static int MapWidth = 0;
    static int MapHeight = 0;

    static float CameraPosX = 0.0f;
    static float CameraPosY = 0.0f;
    static float TargetCameraPosX = 0.0f;
    static float TargetCameraPosY = 0.0f;

    static float Zoom = 1.0f;
    static float MaxZoom = 3.0f;
    static bool PhysicsPaused = false;
    static bool UserHasControl = true;

    static Entity ControlWorm = null!;
    static Entity CameraTracking = null!;

    static void Load()
    {
        LoadAssets();
        LoadModels();
        var config = gameManager.Conn.Db.Config.Id.Find(0);
        if (config is null)
            throw new Exception("Unable to get config from server!");

        MapWidth = config.MapWidth;
        MapHeight = config.MapHeight;

        CameraPosY = (MapHeight - TARGET_HEIGHT) / 2;
        CameraPosX = (MapWidth - TARGET_WIDTH) / 4;

        Map = new byte[MapWidth * MapHeight];
        CreateMap();

        Entities = gameManager.Conn.Db.Entities.Iter().ToList();
    }
    static void Tick()
    {
        skygif.Update();
        UpdateMusicStream(music);
        float elapsedTime = GetFrameTime();

        HandleUserInput(elapsedTime);

        float viewWidth = TARGET_WIDTH / Zoom;
        float viewHeight = TARGET_HEIGHT / Zoom;


        if (CameraTracking != null)
        {
            CameraPosX = CameraTracking.Position.X - viewWidth / 2;
            CameraPosY = CameraTracking.Position.Y - viewHeight / 2;
            if (CameraTracking.Dead) CameraTracking = null;
        }

        //clamp camera
        if (CameraPosX < 0) CameraPosX = 0; //remove this line to make terrain inf going left somehow
        if (CameraPosX > MapWidth - viewWidth) CameraPosX = MapWidth - viewWidth;
        if (CameraPosY < 0) CameraPosY = 0;
        if (CameraPosY > MapHeight - viewHeight) CameraPosY = MapHeight - viewHeight;

        // fix pixels sometimes disapearing because of floating point stuff
        CameraPosX = MathF.Round(CameraPosX);
        CameraPosY = MathF.Round(CameraPosY);


        if (ControlWorm != null)
        {
            if (ControlWorm.Stable)
            { 
                HandleWormControl(elapsedTime);
            }
        }

        if (PhysicsPaused)
        {
            if (IsKeyDown(KeyboardKey.O))
                HandlePhysics(elapsedTime);
        }
        else
        {
            //do 10 physics steps
            for (int i = 0; i < 10; i++)
            {
                HandlePhysics(elapsedTime);
            }
        }
    }

    static float[] GenerateLayer(float start = 0.5f, int octaves = 8, float bias = 2.0f)
    {
        float[] layer = new float[MapWidth];
        float[] NoiseSeed = new float[MapWidth];

        for (int i = 0; i < MapWidth; i++)
            NoiseSeed[i] = Random.Shared.NextSingle();

        NoiseSeed[0] = start;
        PerlinNoise1D(MapWidth, NoiseSeed, octaves, bias, ref layer);
        return layer;
    }
    static void CreateMap()
    {
        float[] Clouds = GenerateLayer(0.01f);
        float[] Surface = GenerateLayer();
        float[] Rocks = GenerateLayer(0.9f, 10);


        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                byte mapColor = (Random.Shared.Next(500) == 1) ? (byte)MapColor.Cloud :(byte)MapColor.Skyblue;

                if (y >= Clouds[x] * MapHeight)
                { 
                    mapColor = (int)MapColor.Skyblue;
                }

                if (y >= Surface[x] * MapHeight)
                {
                    var rng = Random.Shared.Next(10);
                    mapColor = rng switch 
                    {
                        1 => (byte)MapColor.Grass2,
                        2 => (byte)MapColor.Grass2,
                        3 => (byte)MapColor.Grass2,
                        4 => (byte)MapColor.Grass2,
                        5 => (byte)MapColor.Grass2,
                        _ => (byte)MapColor.Grass1,
                    }; 
                }

                if (y >= Rocks[x] * MapHeight)
                { 
                    mapColor = (Random.Shared.Next(10) == 1) ? (byte)MapColor.Rock2 :(byte)MapColor.Rock1;
                }
                Map[y * MapWidth + x] = mapColor;
            }
        }
    }

    static bool TryGetMouseWorldPos(out Vector2 output)
    {
        output = Vector2.Zero;
        Vector2 mouse = GetMousePosition();

        int windowWidth = GetScreenWidth();
        int windowHeight = GetScreenHeight();

        // 1) “Base” zoom that you always use first
        float baseZoom = windowWidth / (float)TARGET_WIDTH;

        // 2) Size of the render texture before applying your extra Zoom
        float preZoomW = TARGET_WIDTH * baseZoom;
        float preZoomH = TARGET_HEIGHT * baseZoom;

        // 3) Center it on-screen using those pre-zoom sizes
        float offsetX = (windowWidth - preZoomW) / 2f;
        float offsetY = (windowHeight - preZoomH) / 2f;

        // 4) Combine with your camera Zoom factor
        float fullZoom = baseZoom * Zoom;

        // 5) Final on-screen size (for hit-testing)
        float destW = preZoomW * Zoom;
        float destH = preZoomH * Zoom;

        // 6) Is the mouse inside that rectangle?
        if (mouse.X < offsetX || mouse.X > offsetX + destW ||
            mouse.Y < offsetY || mouse.Y > offsetY + destH)
            return false; // mouse outside render area :(

        // 7) Map screen→render-texture coords
        float renderX = (mouse.X - offsetX) / fullZoom;
        float renderY = (mouse.Y - offsetY) / fullZoom;

        // 8) Then render-texture→world coords
        output = new Vector2(renderX + CameraPosX,
                             renderY + CameraPosY);
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
