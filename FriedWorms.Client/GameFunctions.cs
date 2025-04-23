using Raylib_cs;
using static Raylib_cs.Raylib;
using SpacetimeDB.Types;

namespace FriedWorms.Client;

partial class Program
{
    static void CreateCircle(int xc, int yc, int r)
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
                    Map[ny * MapWidth + i] = 0;
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
    static void CreateExplosion(float worldX, float worldY, float radius)
    {

        CreateCircle((int)worldX, (int)worldY, (int)radius);

        //blow other entities away
        foreach (var entity in Entities)
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
            }
        }

        for (int i = 0; i < (int)radius; i++)
        {
            Entities.Add(CreateEntityDebris(new DbVector2(worldX, worldY)));
        }
    }
}
