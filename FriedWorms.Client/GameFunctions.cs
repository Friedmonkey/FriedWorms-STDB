using Raylib_cs;
using static Raylib_cs.Raylib;
using SpacetimeDB.Types;
using System;

namespace FriedWorms.Client;

partial class Program
{
    static List<byte> CreateCircle(int xc, int yc, int r, byte fill = 0)
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

                if (sx >= 0 && sx < MapWidth && sy >= 0 && sy < MapHeight)
                {
                    byte val = Map[sy * MapWidth + sx];
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
                if (ny >= 0 && ny < MapHeight && i >= 0 && i < MapWidth)
                    Map[ny * MapWidth + i] = fill;
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

    static void CreateExplosion(float worldX, float worldY, float radius, float baseDamage, float damageFalloff = 1.5f)
    {
        explosions.Play();

        var colorIndices = CreateCircle((int)worldX, (int)worldY, (int)radius);

        var currentEntities = Entities.Where(e => !e.Dead).ToList();

        //blow other entities away
        foreach (var entity in currentEntities)
        {
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

                entity.Damage(damage);

            }
        }

        for (int i = 0; i < radius/2; i++)
        {
            Entities.Add(CreateEntitySmoke(new DbVector2(worldX, worldY)));
        }

        foreach (byte colorIdx in colorIndices) 
        {
            Entities.Add(CreateEntityDebris(new DbVector2(worldX, worldY), colorIdx));
        }
    }
}
