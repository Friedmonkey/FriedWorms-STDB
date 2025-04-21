using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace FriedWorms.Client;

public partial class Program
{
    static void LoadModels()
    {
        dummy = DefineDummy();
        missile = DefineMissile();
    }
    static List<Vector2> DefineMissile(float radius = 2.5f)
    {
        // Defines a rocket like shape
        List<Vector2> vecModel = new();

        vecModel.Add(new (0.0f, 0.0f));    
        vecModel.Add(new (1.0f, 1.0f));
        vecModel.Add(new (2.0f, 1.0f));
        vecModel.Add(new (2.5f, 0.0f));
        vecModel.Add(new (2.0f, -1.0f));
        vecModel.Add(new (1.0f, -1.0f));
        vecModel.Add(new (0.0f, 0.0f));
        vecModel.Add(new (-1.0f, -1.0f));
        vecModel.Add(new (-2.5f, -1.0f));
        vecModel.Add(new (-2.0f, 0.0f));
        vecModel.Add(new (-2.5f, 1.0f));
        vecModel.Add(new (-1.0f, 1.0f));

        // Scale points to make shape unit sized
        vecModel = vecModel.Select(v =>
            new Vector2(v.X * radius, v.Y * radius)
        ).ToList();
        return vecModel;
    }
    static List<Vector2> DefineDummy(float radius = 4.0f)
    {
        // Defines a circle with a line from center to edge
        List<Vector2> vecModel = new();
        vecModel.Add(new Vector2(0.0f, 0.0f)); // center

        for (int i = 0; i < 10; i++)
        {
            float angle = i / 9.0f * 2.0f * MathF.PI;
            float x = MathF.Cos(angle) * radius;
            float y = MathF.Sin(angle) * radius;
            vecModel.Add(new Vector2(x, y));
        }

        return vecModel;
    }


    static List<Vector2> missile;
    static List<Vector2> dummy;
    static readonly List<Vector2> shape = new()
    {
        new Vector2(-4, -4),
        new Vector2(4, -4),
        new Vector2(4, 4),
        new Vector2(-4, 4),
    };
}
