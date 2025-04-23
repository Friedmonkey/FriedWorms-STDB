using Raylib_cs;
using static Raylib_cs.Raylib;
using SpacetimeDB.Types;

namespace FriedWorms.Client;

partial class Program
{
    static void CreateExplosion(float worldX, float worldY, float radius)
    {
        //todo blow other entities away


        for (int i = 0; i < (int)radius; i++)
        {
            Entities.Add(CreateEntityDebris(new DbVector2(worldX, worldY)));
        }
    }
}
