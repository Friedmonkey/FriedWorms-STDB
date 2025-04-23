using Raylib_cs;
using static Raylib_cs.Raylib;
using SpacetimeDB.Types;
using System.Numerics;

namespace FriedWorms.Client;

public enum EntityModelType : uint
{ 
    None = 0,
    Worm = 1,
    Missile = 2,
    Dummy = 3,
    Debris = 4,
    Gravestone = 5,
    Granade = 6,
}
public static partial class Program
{

    public static readonly Color Green = new Color(0, 228, 48, 255);
    public static readonly Color DarkGreen = new Color(0, 117, 44, 255);
    public static readonly Color GrenadeGreen = new Color(0, 90, 40, 255);

    public static void Draw(this Entity entity)
    {
        (List<Vector2> model, Color color) drawObj = ((EntityModelType)entity.ModelData) switch
        {
            EntityModelType.None => (shape, Color.Magenta),
            EntityModelType.Worm => (worm, Color.Pink),
            EntityModelType.Missile => (missile, Color.Yellow),
            EntityModelType.Dummy => (dummy, Color.White),
            EntityModelType.Debris => (debris, Color.DarkGreen),
            EntityModelType.Granade => (granade, GrenadeGreen),
            EntityModelType.Gravestone => (gravestone, Color.DarkGray),
            _ => throw new Exception($"Unknow model data {entity.ModelData}")
        };

        DrawWireFrameModel(drawObj.model, entity.Position.X, entity.Position.Y, MathF.Atan2(entity.Velocity.Y, entity.Velocity.X), 1.0f, drawObj.color);
    }
    public static void Damage(this Entity entity, float damage)
    {
        if (entity.MaxHealth == 0) //entity cant take damage
            return;

        entity.Health -= damage;
        if (entity.Health <= 0)
        {
            entity.Dead = true;
            entity.OnDeath();
        }
    }
    public static void OnDeath(this Entity entity)
    {
        switch ((EntityModelType)entity.ModelData)
        {
            case EntityModelType.Missile:
                CreateExplosion(entity.Position.X, entity.Position.Y, 20.0f, 85, 0.2f);
                break;
            case EntityModelType.Granade:
                CreateExplosion(entity.Position.X, entity.Position.Y, 10.0f, 20, 0.2f);
                break;
            case EntityModelType.Worm:
                Entities.Add(CreateEntityGravestone(entity.Position));
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
            Friction = 0.8f,
            MaxHealth = 150,
            Health = 50
        };
    }
    public static Entity CreateEntityWorm(DbVector2 position)
    {
        return new Entity()
        {
            ModelData = (uint)EntityModelType.Worm,
            Position = position,
            Radius = 2.5f,
            Friction = 0.2f,
            MaxHealth = 100.0f,
            Health = 100.0f
        };
    }
    public static Entity CreateEntityGravestone(DbVector2 position)
    {
        return new Entity()
        {
            ModelData = (uint)EntityModelType.Gravestone,
            Position = position,
            Radius = 4f,
            Friction = 0.2f
        };
    }
    public static Entity CreateEntityGranade(DbVector2 position)
    {
        return new Entity()
        {
            ModelData = (uint)EntityModelType.Granade,
            Position = position,
            Radius = 1f,
            Friction = 0.8f,
            MaxBounceCount = 3
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