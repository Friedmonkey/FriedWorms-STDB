using System.Numerics;

namespace FriedWorms.Client;

public partial class Program
{
    static void LoadModels()
    {
        dummy = DefineDummy();
        missile = DefineMissile();
        worm = DefineWorm();
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
    static List<Vector2> DefineWorm(float radius = 4.0f)
    {
        List<Vector2> vecModel = new();

        int segments = 6; // how many "body chunks"
        float width = 1.0f;
        float height = 1.0f;

        // Centering offset
        float totalLength = segments * height;
        float startY = -totalLength / 2;

        for (int i = 0; i < segments; i++)
        {
            float y = startY + i * height;

            // Wiggle left and right like a worm :3
            float xOffset = (i % 2 == 0) ? -0.2f : 0.2f;

            vecModel.Add(new Vector2(-width / 2 + xOffset, y));
            vecModel.Add(new Vector2(width / 2 + xOffset, y));
        }

        // Close the shape (go backward through the same Y points)
        for (int i = segments - 1; i >= 0; i--)
        {
            float y = startY + i * height;
            float xOffset = (i % 2 == 0) ? -0.2f : 0.2f;

            vecModel.Add(new Vector2(width / 2 + xOffset, y + height));
            vecModel.Add(new Vector2(-width / 2 + xOffset, y + height));
        }

        return vecModel;
    }

    static List<Vector2> missile;
    static List<Vector2> dummy;
    static List<Vector2> worm;
    static readonly List<Vector2> gravestone = new()
    {
        // Bottom rectangle part
        new Vector2(-4, -6),
        new Vector2(4, -6),
        new Vector2(4, 0),

        // Half-circle top (semi-arc from right to left)
        new Vector2(3.5f, 2.0f),
        new Vector2(2.5f, 3.2f),
        new Vector2(1.2f, 3.8f),
        new Vector2(0, 4),
        new Vector2(-1.2f, 3.8f),
        new Vector2(-2.5f, 3.2f),
        new Vector2(-3.5f, 2.0f),

        // Back down left side
        new Vector2(-4, 0),
        new Vector2(-4, -6), // Close the loop
    };


    static readonly List<Vector2> grenade = new()
    {
        new Vector2(-2.0f, -2.0f),
        new Vector2(2.0f, -2.0f),
        new Vector2(2.0f, 2.0f),
        new Vector2(0.0f, 3.0f), // Sticking out point (like a fuse or something cool)
        new Vector2(-2.0f, 2.0f),
    };

    static readonly List<Vector2> debris = new()
    {
        new Vector2(-0.5f, -0.5f),
        new Vector2(0.5f, -0.5f),
        new Vector2(0.5f, 0.5f),
        new Vector2(-0.5f, 0.5f),
    };
    static readonly List<Vector2> shape = new()
    {
        new Vector2(-4, -4),
        new Vector2(4, -4),
        new Vector2(4, 4),
        new Vector2(-4, 4),
    };
}
