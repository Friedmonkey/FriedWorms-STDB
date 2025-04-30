using static Module;

namespace SpacetimeDB;

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
    GrassGranade = 8,
    Airstrike = 9,
}

public static partial class Module
{
    public static void Damage(this Entity entity, ReducerContext ctx, Config config, float damage)
    {
        if (entity.MaxHealth == 0) //entity cant take damage
            return;

        entity.Health -= damage;
        if (entity.Health <= 0)
        {
            entity.Dead = true;
            entity.OnDeath(ctx, config);
        }
    }
    public static void OnTick(this Entity entity, ReducerContext ctx, float elapsedTime)
    {
        //bool random(int chance) => (DeterministicRandom.Next((int)(chance / elapsedTime)) == 1);
        switch ((EntityModelType)entity.ModelData)
        {
            case EntityModelType.Missile:
                var smoke = CreateEntitySmoke(entity.Position);
                smoke.DeathTimer = 1f;
                ctx.Db.Entities.Insert(smoke);
                break;
            default:
                break;
        }
    }
    public static void OnDeath(this Entity entity, ReducerContext ctx, Config config)
    {
        switch ((EntityModelType)entity.ModelData)
        {
            case EntityModelType.Missile:
                CreateExplosion(ctx, config, entity.Position.X, entity.Position.Y, 20.0f, 85, 0.2f);
                break;
            case EntityModelType.Granade:
                CreateExplosion(ctx, config, entity.Position.X, entity.Position.Y, 15.0f, 90, 0.1f);
                break;
            //case EntityModelType.GrassGranade:
            //    CreateImplosion(entity.Position.X, entity.Position.Y, 15.0f, 10, 0.1f);
            //    break;
            case EntityModelType.Worm:
                {
                    var gravestone = CreateEntityGravestone(entity.Position);
                    gravestone.Velocity = entity.Velocity;
                    gravestone.Velocity.Y = -MathF.Abs(gravestone.Velocity.Y) - 15f;
                    ctx.Db.Entities.Insert(gravestone);
                    for (int i = 0; i < 5; i++)
                    {
                        ctx.Db.Entities.Insert(CreateEntityDebris(entity.Position, (byte)MapColor.Worm));
                    }
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
            case EntityModelType.GrassGranade: return CreateEntityGrassGranade(position);
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
            DeathTimer = float.PositiveInfinity,
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
            DeathTimer = float.PositiveInfinity,
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
            DeathTimer = float.PositiveInfinity,
        };
    }
    public static Entity CreateEntityGrassGranade(DbVector2 position)
    {
        var granade = CreateEntityGranade(position);
        granade.ModelData = (byte)EntityModelType.GrassGranade;
        granade.MaxBounceCount = 1;
        return granade;
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
            DeathTimer = float.PositiveInfinity,
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
            DeathTimer = float.PositiveInfinity,
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
            DeathTimer = float.PositiveInfinity,
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
            ExtraGravityForce = -2.1f,
            DeathTimer = rnd()*8
        };
    }
}