using FriedWorms.Common;
using SpacetimeDB;

public static partial class Module
{
    [Table(Name = "Config", Public = true)]
    public partial struct Config
    {
        [PrimaryKey]
        public uint Id;

        public int MapWidth;
        public int MapHeight;

        public uint ControlWormId;
        public uint CameraTrackingId;

        public int RandomSeed;
    }
    [Table(Name = "Game", Public = false)]
    public partial struct Game
    {
        [PrimaryKey]
        public uint Id;

        public List<byte> Map;

        public byte CurrentState;
        public byte NextState;

        public bool GameIsStable;
        public bool PlayerActionComplete;
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

    [Table(Name = "Explosions", Public = true)]
    public partial struct Explosion
    {
        [PrimaryKey, AutoInc]
        public uint Id;

        public DbVector2 Position;
        public float Radius;
        public float BaseDamage;
        public float DamageFalloff;
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

        //[SpacetimeDB.Index.BTree]
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

        var game = new Game()
        {
            CurrentState = (byte)GameState.Idle,
            NextState = (byte)GameState.Idle,
            GameIsStable = false,
            PlayerActionComplete = false,
            Map = new(new byte[config.MapWidth * config.MapHeight])
        };

        ctx.Db.Config.Insert(config);
        ctx.Db.Game.Insert(game);
        CreateMap(ctx);

        ctx.Db.physics_schedule.Insert(new()
        {
            ScheduledAt = new ScheduleAt.Interval(TimeSpan.FromMilliseconds(5))
        });
    }


    [Reducer]
    public static void CreateMap(ReducerContext ctx)
    {
        var config = ctx.Db.Config.Id.Find(0) ?? throw new Exception("no config");
        var game = ctx.Db.Game.Id.Find(0) ?? throw new Exception("no game");
        var DeterministicRandom = new Random(config.RandomSeed);

        MapHandeler.CreateMap(DeterministicRandom, ref game.Map, config.MapWidth, config.MapHeight);

        ctx.Db.Config.Id.Update(config);
        ctx.Db.Game.Id.Update(game);
        Log.Info("Map has been created! with " + game.Map.Distinct().Count() + "unique");
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

    //[Reducer]
    //public static void Join(ReducerContext ctx)
    //{ 
        
    //}
    //[Reducer]
    //public static void Leave(ReducerContext ctx)
    //{

    //}
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
        Log.Info("Entities cleared!");
    }

    [Reducer]
    public static void Reset(ReducerContext ctx)
    {
        foreach (var entity in ctx.Db.Entities.Iter())
            ctx.Db.Entities.Delete(entity);
        foreach (var explosion in ctx.Db.Explosions.Iter())
            ctx.Db.Explosions.Delete(explosion);
        foreach (var player in ctx.Db.Players.Iter())
            ctx.Db.Players.Delete(player);
        foreach (var player in ctx.Db.LoggedOutPlayers.Iter())
            ctx.Db.LoggedOutPlayers.Delete(player);

        var config = ctx.Db.Config.Id.Find(0) ?? throw new Exception("no config found!");
        config.RandomSeed = Random.Shared.Next();
        config.CameraTrackingId = 0;
        config.ControlWormId = 0;

        ctx.Db.Config.Id.Update(config);

        CreateMap(ctx);

        Log.Info("Reset complete!");
    }
}
