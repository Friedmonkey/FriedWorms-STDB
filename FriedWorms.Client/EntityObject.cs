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
    Smoke = 7,
}
public static partial class Program
{

    public static readonly Color Green = new Color(0, 228, 48, 255);
    public static readonly Color DarkGreen = new Color(0, 117, 44, 255);
    public static readonly Color GrenadeGreen = new Color(0, 90, 40, 255);
    public static readonly Color DarkGreen2 = new Color(0, 114, 42, 255);

    public static void Draw(this Entity entity, bool uiscaling = false)
    {
        (List<Vector2> model, Color color, bool clamped) drawObj = ((EntityModelType)entity.ModelData) switch
        {   
            EntityModelType.None => (shape, Color.Magenta, false),
            EntityModelType.Worm => (worm, Color.Pink, true),
            EntityModelType.Missile => (missile, Color.Yellow, false),
            EntityModelType.Dummy => (dummy, Color.White, false),
            EntityModelType.Debris => (debris, Color.DarkGreen, false),
            EntityModelType.Granade => (grenade, GrenadeGreen, false),
            EntityModelType.Gravestone => (gravestone, Color.DarkGray, true),
            EntityModelType.Smoke => (debris, Color.LightGray, false),
            _ => throw new Exception($"Unknow model data {entity.ModelData}")
        };
        var rotation = MathF.Atan2(entity.Velocity.Y, entity.Velocity.X);

        if (drawObj.clamped && entity.Stable)
        {
            rotation = MathF.Abs(rotation);
            rotation = Math.Clamp(rotation, 3 - 0.2f, 3 + 0.2f);
        }

        if (entity.CustomColorIndex != 0)
            drawObj.color = GetMapColor((MapColor)entity.CustomColorIndex);

        DrawWireFrameModel(drawObj.model, entity.Position.X, entity.Position.Y, rotation, 1.0f, drawObj.color, uiscaling);
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
                CreateExplosion(entity.Position.X, entity.Position.Y, 15.0f, 90, 0.1f);
                break;
            case EntityModelType.Worm:
                {
                    var gravestone = CreateEntityGravestone(entity.Position);
                    gravestone.Velocity = entity.Velocity;
                    gravestone.Velocity.Y = -MathF.Abs(gravestone.Velocity.Y) - 15f;
                    Entities.Add(gravestone);
                }
                break;
            default:
                break;
        }
    }

    public static Entity CreateEntity(DbVector2 position, EntityModelType entityType, byte colorIndex = 0)
    {
        switch (entityType)
        {
            case EntityModelType.Worm: return CreateEntityWorm(position);
            case EntityModelType.Missile: return CreateEntityMissile(position);
            case EntityModelType.Dummy: return CreateEntityDummy(position);
            case EntityModelType.Debris: return CreateEntityDebris(position, colorIndex);
            case EntityModelType.Gravestone: return CreateEntityGravestone(position);
            case EntityModelType.Granade: return CreateEntityGranade(position);
            case EntityModelType.Smoke: return CreateEntitySmoke(position, colorIndex);
            default: throw new Exception($"Unknow model data {entityType}");
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
            Health = 50,
            ShootingAngle = float.NegativeZero,
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
            Health = 100.0f,
            ShootingAngle = float.NegativeZero,
        };
    }
    public static Entity CreateEntityGravestone(DbVector2 position)
    {
        return new Entity()
        {
            ModelData = (uint)EntityModelType.Gravestone,
            Position = position,
            Radius = 4f,
            Friction = 0.2f,
            ShootingAngle = float.NegativeZero,
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
            MaxBounceCount = 3,
            ShootingAngle = float.NegativeZero,
        };
    }
    public static Entity CreateEntityMissile(DbVector2 position)
    {
        return new Entity()
        {
            ModelData = (uint)EntityModelType.Missile,
            Position = position,
            Radius = 3.5f,
            Friction = 0.8f,
            MaxBounceCount = 1, //after one bounce it dies (explodes too!)
            ShootingAngle = float.NegativeZero,
        };
    }
    public static Entity CreateEntityDebris(DbVector2 position, byte colorIndex = 0)
    {
        float rnd() => (Random.Shared.NextSingle() * 2 * MathF.PI);
        return new Entity()
        {
            ModelData = (uint)EntityModelType.Debris,
            Position = position,
            Velocity = new(10 * MathF.Cos(rnd()), 10 * MathF.Sin(rnd())),
            MaxBounceCount = 5,
            Radius = 0.8f,
            Friction = 0.8f,
            ShootingAngle = float.NegativeZero,
            CustomColorIndex = colorIndex,
        };
    }
    public static Entity CreateEntitySmoke(DbVector2 position, byte colorIndex = 0)
    {
        float rnd() => (Random.Shared.NextSingle() * 2 * MathF.PI);
        return new Entity()
        {
            ModelData = (uint)EntityModelType.Smoke,
            Position = position,
            Velocity = new(3 * MathF.Cos(rnd()), 4 * MathF.Sin(rnd())),
            MaxBounceCount = 2,
            Radius = 1.2f,
            Friction = 0.2f,
            ShootingAngle = float.NegativeZero,
            CustomColorIndex = colorIndex,
            ExtraGravityForce = -2.1f
        };
    }
}