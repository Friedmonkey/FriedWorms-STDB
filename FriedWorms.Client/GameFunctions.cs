using Raylib_cs;
using static Raylib_cs.Raylib;
using SpacetimeDB.Types;

namespace FriedWorms.Client;

partial class Program
{
    static void CreateCircle(int xc, int yc, int r, byte fill = 0)
    {
        // Taken from wikipedia
        int x = 0;
        int y = r;
        int p = 3 - 2 * r;
        if (r == 0) return;

        void drawline(int sx, int ex, int ny)
        {
            for (int i = sx; i < ex; i++)
                if (ny >= 0 && ny < MapHeight && i >= 0 && i < MapWidth)
                    Map[ny * MapWidth + i] = fill;
        };

        while (y >= x)
        {
            // Modified to draw scan-lines instead of edges
            drawline(xc - x, xc + x, yc - y);
            drawline(xc - y, xc + y, yc - x);
            drawline(xc - x, xc + x, yc + y);
            drawline(xc - y, xc + y, yc + x);
            if (p < 0) p += 4 * x++ + 6;
            else p += 4 * (x++ - y--) + 10;
        }
    }
    static void CreateExplosion(float worldX, float worldY, float radius, float baseDamage, float damageFalloff = 1.5f)
    {
        explosions.Play();
        CreateCircle((int)worldX, (int)worldY, (int)radius);

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

        for (int i = 0; i < (int)radius; i++)
        {
            Entities.Add(CreateEntityDebris(new DbVector2(worldX, worldY)));
        }
    }
}
