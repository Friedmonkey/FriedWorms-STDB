using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FriedWorms.Client;

public partial class Program
{
    static void LoadModels()
    {
        dummy = DefineDummy();
    }
    static List<Vector2> DefineDummy(float radius = 5.0f)
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


    static List<Vector2> dummy;
    static readonly List<Vector2> shape = new()
    {
        new Vector2(-4, -4),
        new Vector2(4, -4),
        new Vector2(4, 4),
        new Vector2(-4, 4),
    };
}
