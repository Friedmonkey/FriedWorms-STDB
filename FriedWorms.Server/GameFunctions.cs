using FriedWorms.Common;
using static Module;

namespace SpacetimeDB;

public static partial class Module
{

    static void CreateExplosion(ReducerContext ctx, Config config, Game game, float worldX, float worldY, float radius, float baseDamage, float damageFalloff = 1.5f)
    {
        var colorIndices = MapHandeler.CreateCircle(ref game.Map, config.MapWidth, config.MapHeight, (int)worldX, (int)worldY, (int)radius);
        ctx.Db.Explosions.Insert(new Explosion()
        {
            Position = new DbVector2(worldX, worldY),
            Radius = radius,

            BaseDamage = baseDamage,
            DamageFalloff = damageFalloff,
        });

        var currentEntities = ctx.Db.Entities.Iter().Where(e => !e.Dead).ToList();

        //blow other entities away
        for (int i = 0; i < currentEntities.Count(); i++)
        {
            Entity entity = currentEntities[i];
            float dx = entity.Position.X - worldX;
            float dy = entity.Position.Y - worldY;
            float distance = MathF.Sqrt(dx*dx + dy*dy);

            if (distance < 0.0001f)
            { 
                //prevent setting the object velocity to infinity
                //which will blast the entity out of space and time, which may confuse the player.
                distance = 0.0001f; 
            }

            if (distance < radius)
            {
                entity.Velocity.X = (dx / distance) * radius;
                entity.Velocity.Y = (dy / distance) * radius;
                entity.Stable = false;

                if (entity.MaxHealth == 0) //they cant take damage, dont bother calculating
                    continue;

                //entity take damage
                // Proximity = 1 at center, 0 at edge
                var proximity = 1.0f - (distance / radius);
                proximity = Math.Clamp(proximity, 0.0f, 1.0f);

                // Apply falloff (e.g. 1 = linear, 2 = fast falloff, 0.5 = soft)
                float scaledProximity = MathF.Pow(proximity, damageFalloff);

                // Final damage
                float damage = baseDamage * scaledProximity;

                entity.Damage(ctx, config, game, damage);
                ctx.Db.Entities.Id.Update(entity);
            }
        }

        //for (int i = 0; i < radius/2; i++)
        //{
        //    ctx.Db.Entities.Insert(CreateEntitySmoke(new DbVector2(worldX, worldY)));
        //}

        //foreach (byte colorIdx in colorIndices) 
        //{
        //    ctx.Db.Entities.Insert(CreateEntityDebris(new DbVector2(worldX, worldY), colorIdx));
        //}
    }
    //static void CreateImplosion(float worldX, float worldY, float radius, float baseDamage, float damageFalloff = 1.5f)
    //{
    //    implosions.Play();

    //    for (int i = 0; i < radius / 2; i++)
    //    {
    //        Entities.Add(CreateEntityDebris(new DbVector2(worldX, worldY), (byte)MapColor.Grass1));
    //    }

    //    var colorIndices = CreateCircle((int)worldX, (int)worldY, (int)radius, (byte)MapColor.Grass1);

    //    var currentEntities = Entities.Where(e => !e.Dead).ToList();

    //    //blow other entities away
    //    foreach (var entity in currentEntities)
    //    {
    //        float dx = entity.Position.X - worldX;
    //        float dy = entity.Position.Y - worldY;
    //        float distance = MathF.Sqrt(dx * dx + dy * dy);

    //        if (distance < 0.0001f)
    //        {
    //            //prevent setting the object velocity to infinity
    //            //which will blast the entity out of space and time, which may confuse the player.
    //            distance = 0.0001f;
    //        }

    //        if (distance < radius)
    //        {
    //            entity.Velocity.X = (dx / distance) * radius/2;
    //            entity.Velocity.Y = (dy / distance) * radius/2;
    //            entity.Stable = false;

    //            if (entity.MaxHealth == 0) //they cant take damage, dont bother calculating
    //                continue;

    //            //entity take damage
    //            // Proximity = 1 at center, 0 at edge
    //            var proximity = 1.0f - (distance / radius);
    //            proximity = Math.Clamp(proximity, 0.0f, 1.0f);

    //            // Apply falloff (e.g. 1 = linear, 2 = fast falloff, 0.5 = soft)
    //            float scaledProximity = MathF.Pow(proximity, damageFalloff);

    //            // Final damage
    //            float damage = baseDamage * scaledProximity;

    //            entity.Damage(damage);
    //        }
    //    }
    //}
}
