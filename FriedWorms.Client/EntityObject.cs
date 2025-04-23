using Raylib_cs;
using static Raylib_cs.Raylib;
using SpacetimeDB.Types;

namespace FriedWorms.Client;

public enum EntityModelType : uint
{ 
    None = 0,
    Worm = 1,
    Missile = 2,
    Dummy = 3,
    Debris = 4,
}
public static partial class Program
{
    public static void Draw(this Entity entity)
    {
        switch ((EntityModelType)entity.ModelData)
        {
            case EntityModelType.None:
                DrawWireFrameModel(shape, entity.Position.X, entity.Position.Y, MathF.Atan2(entity.Velocity.Y, entity.Velocity.X), 1.0f, Color.Magenta);
                break;
            case EntityModelType.Worm:
                DrawWireFrameModel(shape, entity.Position.X, entity.Position.Y, MathF.Atan2(entity.Velocity.Y, entity.Velocity.X), 1.0f, Color.Red);
                break;
            case EntityModelType.Missile:
                DrawWireFrameModel(missile, entity.Position.X, entity.Position.Y, MathF.Atan2(entity.Velocity.Y, entity.Velocity.X  ), 1.0f, Color.Red);
                break;
            case EntityModelType.Dummy:
                DrawWireFrameModel(dummy, entity.Position.X, entity.Position.Y, MathF.Atan2(entity.Velocity.Y, entity.Velocity.X), 1.0f, Color.White);
                break;
            case EntityModelType.Debris:
                DrawWireFrameModel(debris, entity.Position.X, entity.Position.Y, MathF.Atan2(entity.Velocity.Y, entity.Velocity.X), 1.0f, Color.DarkGreen);
                break;
            default:
                throw new Exception($"Unknow model data {entity.ModelData}");
        }
    }
    public static void OnDeath(this Entity entity)
    {
        switch ((EntityModelType)entity.ModelData)
        {
            case EntityModelType.Missile:
                CreateExplosion(entity.Position.X, entity.Position.Y, 20.0f);
                break;
            default:
                break;
        }
    }

    public static Entity CreateEntityDummy(DbVector2 position)
    {
        return new Entity()
        {
            ModelData = (uint)EntityModelType.Dummy,
            Position = position,
            Radius = 4.0f,
            Friction = 0.8f
        };
    }
    public static Entity CreateEntityWorm(DbVector2 position)
    {
        return new Entity()
        {
            ModelData = (uint)EntityModelType.Worm,
            Position = position,
            Radius = 4.0f,
            Friction = 0.8f
        };
    }
    public static Entity CreateEntityMissile(DbVector2 position)
    {
        return new Entity()
        {
            ModelData = (uint)EntityModelType.Missile,
            Position = position,
            Radius = 4.0f,
            Friction = 0.8f,
            MaxBounceCount = 1 //after one bounce it dies (explodes too!)
        };
    }
    public static Entity CreateEntityDebris(DbVector2 position)
    {
        float rnd() => (Random.Shared.NextSingle() * 2 * MathF.PI);
        return new Entity()
        {
            ModelData = (uint)EntityModelType.Debris,
            Position = position,
            Velocity = new(10 * MathF.Cos(rnd()), 10 * MathF.Sin(rnd())),
            MaxBounceCount = 5,
            Radius = 0.8f,
            Friction = 0.8f
        };
    }
}