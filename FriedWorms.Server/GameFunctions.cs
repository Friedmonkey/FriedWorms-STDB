using static Module;

namespace SpacetimeDB;

public static partial class Module
{
    static List<byte> CreateCircle(Config config, int xc, int yc, int r, byte fill = 0)
    {
        const int defaultSample = 5;
        List<byte> sampled = new(defaultSample + r); // max size r

        void Sample(int radius, int multiplier = 1)
        { 
            // Sample edge before drawing
            for (int i = 0; i < radius*multiplier; i++)
            {
                double angle = (2 * Math.PI * i) / radius;
                int sx = (int)Math.Round(xc + Math.Cos(angle) * radius);
                int sy = (int)Math.Round(yc + Math.Sin(angle) * radius);

                if (sx >= 0 && sx < config.MapWidth && sy >= 0 && sy < config.MapHeight)
                {
                    byte val = config.Mapp[sy * config.MapWidth + sx];
                    if (val != 0)
                        sampled.Add(val);
                }
            }
        }
        Sample(r);
        if (sampled.Count < 5)
            Sample(defaultSample, 4);

        // Draw the filled circle (unchanged)
        int x = 0;
        int y = r;
        int p = 3 - 2 * r;
        if (r == 0) return sampled;

        void drawline(int sx, int ex, int ny)
        {
            for (int i = sx; i < ex; i++)
                if (ny >= 0 && ny < config.MapHeight && i >= 0 && i < config.MapWidth)
                    config.Mapp[ny * config.MapWidth + i] = fill;
        }

        while (y >= x)
        {
            drawline(xc - x, xc + x, yc - y);
            drawline(xc - y, xc + y, yc - x);
            drawline(xc - x, xc + x, yc + y);
            drawline(xc - y, xc + y, yc + x);
            if (p < 0) p += 4 * x++ + 6;
            else p += 4 * (x++ - y--) + 10;
        }

        return sampled;
    }

    static void CreateExplosion(ReducerContext ctx, Config config, float worldX, float worldY, float radius, float baseDamage, float damageFalloff = 1.5f)
    {
        var colorIndices = CreateCircle(config, (int)worldX, (int)worldY, (int)radius);

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

                entity.Damage(ctx, config, damage);
            }
        }

        for (int i = 0; i < radius/2; i++)
        {
            ctx.Db.Entities.Insert(CreateEntitySmoke(new DbVector2(worldX, worldY)));
        }

        foreach (byte colorIdx in colorIndices) 
        {
            ctx.Db.Entities.Insert(CreateEntityDebris(new DbVector2(worldX, worldY), colorIdx));
        }
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
