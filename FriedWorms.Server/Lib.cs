using SpacetimeDB;

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
public static partial class Module
{
    [Type]
    public enum GameState
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
    //singleton
    [Table(Name = "Config", Public = true)]
    public partial struct Config
    {
        [PrimaryKey]
        public uint Id;

        public int MapWidth;
        public int MapHeight;
        public byte[] Map;

        public uint ControlWormId;
        public uint CameraTrackingId;
        public GameState CurrentState;
        public GameState NextState;

        public bool GameIsStable;
        public bool PlayerActionComplete;

        public int RandomSeed;
    }

    [SpacetimeDB.Type]
    public partial struct DbVector2
    {
        public float X;
        public float Y;
        public DbVector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    [Table(Name = "Entities", Public = true)]
    public partial struct Entity
    {
        [PrimaryKey, AutoInc]
        public uint Id;

        [SpacetimeDB.Index.BTree]
        public uint PlayerId;

        public DbVector2 Position;
        public DbVector2 Velocity;
        public DbVector2 Acceleration;
        public float ShootingAngle;
        public float ExtraGravityForce;

        public float Radius;
        public bool Stable;
        public float Friction;

        public float MaxHealth;
        public float Health;
        public float DeathTimer;

        public int SoundIdx;

        //how many times it can bounce before dying (negative means it dont matter)
        public int MaxBounceCount;
        public bool Dead;
        public byte CustomColorIndex;


        public uint ModelData;
    }

    [Table(Name = "Players", Public = true)]
    [Table(Name = "LoggedOutPlayers")] //same data diffrent private table
    public partial struct Player
    {
        [PrimaryKey]
        public Identity Identity;

        [Unique, AutoInc]
        public uint PlayerId;

        public string Name;
    }

    [Reducer(ReducerKind.Init)]
    public static void Init(ReducerContext ctx)
    {
        Log.Info("Initializing...");
        var config = new Config()
        {
            MapWidth = 1024,
            MapHeight = 512,
            RandomSeed = Random.Shared.Next()
        };

        config.Map = new byte[config.MapWidth * config.MapHeight];
        ctx.Db.Config.Insert(config);
        CreateMap(ctx);
    }

    static float[] GenerateLayer(int width, float start = 0.5f, int octaves = 8, float bias = 2.0f)
    {
        float[] layer = new float[width];
        float[] NoiseSeed = new float[width];

        for (int i = 0; i < width; i++)
            NoiseSeed[i] = Random.Shared.NextSingle();

        NoiseSeed[0] = start;
        PerlinNoise1D(width, NoiseSeed, octaves, bias, ref layer);
        return layer;
    }
    [Reducer]
    public static void CreateMap(ReducerContext ctx)
    {
        var config = ctx.Db.Config.Id.Find(0) ?? throw new Exception("no config");
        //float[] Clouds = GenerateLayer(0.01f);
        float[] Surface = GenerateLayer(config.MapWidth);
        float[] Rocks = GenerateLayer(config.MapWidth, 0.9f, 10);


        for (int x = 0; x < config.MapWidth; x++)
        {
            for (int y = 0; y < config.MapHeight; y++)
            {
                byte mapColor = (int)MapColor.Skyblue;
                //byte mapColor = (DeterministicRandom.Next(500) == 1) ? (byte)MapColor.Cloud :(byte)MapColor.Skyblue;

                //if (y >= Clouds[x] * MapHeight)
                //{ 
                //    mapColor = (int)MapColor.Skyblue;
                //}

                if (y >= Surface[x] * config.MapHeight)
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

                if (y >= Rocks[x] * config.MapHeight)
                {
                    mapColor = (Random.Shared.Next(10) == 1) ? (byte)MapColor.Rock2 : (byte)MapColor.Rock1;
                }
                config.Map[y * config.MapWidth + x] = mapColor;
            }
        }
        Log.Info("Map has been created! with " + config.Map.Distinct().Count() + "unique");
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

    [Reducer(ReducerKind.ClientConnected)]
    public static void Connect(ReducerContext ctx)
    {
        var player = ctx.Db.LoggedOutPlayers.Identity.Find(ctx.Sender);
        if (player is null)
        {
            ctx.Db.Players.Insert(new Player()
            {
                Identity = ctx.Sender,
                Name = "Guest" + Random.Shared.Next(1000, 9999),
            });
            Log.Info("New session created.");
        }
        else
        {
            ctx.Db.Players.Insert(player.Value);
            ctx.Db.LoggedOutPlayers.Identity.Delete(player.Value.Identity);
            Log.Info($"Session found, reusing old session: {ctx.Sender}");
        }
    }

    [Reducer(ReducerKind.ClientDisconnected)]
    public static void Disconnect(ReducerContext ctx)
    { 
        var player = ctx.Db.Players.Identity.Find(ctx.Sender) ?? throw new Exception("Player not found");
        ctx.Db.LoggedOutPlayers.Insert(player);
        ctx.Db.Players.Identity.Delete(player.Identity);
        Log.Info("Player session logged out.");
    }

    [Reducer]
    public static void AddEntity(ReducerContext ctx, Entity entity)
    {
        ctx.Db.Entities.Insert(entity);
        Log.Info($"Added {entity.ModelData.ToString()}");
    }

    [Reducer]
    public static void DeleteEntityByID(ReducerContext ctx, uint entityId)
    {
        ctx.Db.Entities.Id.Delete(entityId);
        Log.Info($"removed entity with id {entityId}");
    }

    [Reducer]
    public static void ClearEntities(ReducerContext ctx)
    {
        foreach (var entity in ctx.Db.Entities.Iter())
        {
            ctx.Db.Entities.Delete(entity);
            Log.Info($"Deleted {entity.ModelData.ToString()}");
        }
        Log.Debug("Entities cleared!");
        Log.Info("Entities cleared!");
        Log.Warn("Entities cleared!");
    }
}
