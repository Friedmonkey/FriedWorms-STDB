using SpacetimeDB;

public static partial class Module
{
    //singleton
    [Table(Name = "Config", Public = true)]
    public partial struct Config
    {
        [PrimaryKey]
        public uint Id;

        public ulong WorldSize;
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
        public DbVector2 Position;
        public DbVector2 Velocity;
        public float Rotation;
    }

    [Table(Name = "Worms", Public = true)]
    public partial struct Worm
    {
        [PrimaryKey, AutoInc]
        public uint Id;

        [SpacetimeDB.Index.BTree]
        public uint PlayerId;
    }

    [Table(Name = "Missiles", Public = true)]
    public partial struct Missile
    {
        [PrimaryKey, AutoInc]
        public uint Id;

        [SpacetimeDB.Index.BTree]
        public uint PlayerId;
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
        ctx.Db.Config.Insert(new Config() 
        {
            WorldSize = 20
        });
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
}
