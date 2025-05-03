namespace FriedWorms.Common;
public static class MapHandeler
{
    public static List<byte> CreateCircle(ref List<byte> map, int width, int height, int xc, int yc, int r, byte fill = 0)
    {
        const int defaultSample = 5;
        List<byte> sampled = new(defaultSample + r); // max size r

        void Sample(ref List<byte> map, int radius, int multiplier = 1)
        {
            // Sample edge before drawing
            for (int i = 0; i < radius * multiplier; i++)
            {
                double angle = (2 * Math.PI * i) / radius;
                int sx = (int)Math.Round(xc + Math.Cos(angle) * radius);
                int sy = (int)Math.Round(yc + Math.Sin(angle) * radius);

                if (sx >= 0 && sx < width && sy >= 0 && sy < height)
                {
                    byte val = map[sy * width + sx];
                    if (val != 0)
                        sampled.Add(val);
                }
            }
        }
        Sample(ref map, r);
        if (sampled.Count < 5)
            Sample(ref map, defaultSample, 4);

        // Draw the filled circle (unchanged)
        int x = 0;
        int y = r;
        int p = 3 - 2 * r;
        if (r == 0) return sampled;

        void drawline(ref List<byte> map, int sx, int ex, int ny)
        {
            for (int i = sx; i < ex; i++)
                if (ny >= 0 && ny < height && i >= 0 && i < width)
                    map[ny * width + i] = fill;
        }

        while (y >= x)
        {
            drawline(ref map, xc - x, xc + x, yc - y);
            drawline(ref map, xc - y, xc + y, yc - x);
            drawline(ref map, xc - x, xc + x, yc + y);
            drawline(ref map, xc - y, xc + y, yc + x);
            if (p < 0) p += 4 * x++ + 6;
            else p += 4 * (x++ - y--) + 10;
        }

        return sampled;
    }
    public static void CreateMap(Random DeterministicRandom, ref List<byte> map, int width, int height)
    {
        float[] Surface = GenerateLayer(DeterministicRandom, width);
        float[] Rocks = GenerateLayer(DeterministicRandom, width, 0.9f, 10);



        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //byte mapColor = (byte)((Random.Shared.Next(10)==1) ? MapColor.Grass1 : MapColor.Grass2);

                //if (y <= config.MapHeight/2)
                //    mapColor = (int)MapColor.Worm;
                byte mapColor = (int)MapColor.Skyblue;
                //byte mapColor = (DeterministicRandom.Next(500) == 1) ? (byte)MapColor.Cloud :(byte)MapColor.Skyblue;

                //if (y >= Clouds[x] * MapHeight)
                //{ 
                //    mapColor = (int)MapColor.Skyblue;
                //}

                if (y >= Surface[x] * height)
                {
                    var rng = DeterministicRandom.Next(10);
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

                if (y >= Rocks[x] * height)
                {
                    mapColor = (DeterministicRandom.Next(10) == 1) ? (byte)MapColor.Rock2 : (byte)MapColor.Rock1;
                }
                map[y * width + x] = mapColor;
            }
        }
    }

    static float[] GenerateLayer(Random DeterministicRandom, int width, float start = 0.5f, int octaves = 8, float bias = 2.0f)
    {
        float[] layer = new float[width];
        float[] NoiseSeed = new float[width];

        for (int i = 0; i < width; i++)
            NoiseSeed[i] = DeterministicRandom.NextSingle();

        NoiseSeed[0] = start;
        PerlinNoise1D(width, NoiseSeed, octaves, bias, ref layer);
        return layer;
    }
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
public enum MapColor
{
    Skyblue = 0,
    Grass1 = 1,
    Grass2 = 2,
    Rock1 = 3,
    Rock2 = 4,

    Worm = 5,


    Unknown = 255
}

public enum GameState : byte
{
    Idle,
    Reset,
    GenerateTerrain,
    GeneratingTerrain,
    DeployUnits,
    DeployingUnits,
    StartPlay,
    CameraMode,
}
