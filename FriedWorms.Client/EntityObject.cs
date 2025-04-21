using Raylib_cs;
using static Raylib_cs.Raylib;
using SpacetimeDB.Types;
using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace FriedWorms.Client;

public enum EntityModelType : uint
{ 
    None = 0,
    Worm = 1,
    Missile = 2,
    Dummy = 3,
}
public static partial class Program
{
    public static void DrawDummy(Entity entity)
    {
        // Inside entity.Draw
        float screenX = (entity.Position.X - CameraPosX) * Zoom;
        float screenY = (entity.Position.Y - CameraPosY) * Zoom;

        // Now draw it
        Raylib.DrawRectangle((int)screenX, (int)screenY, (int)MathF.Ceiling(Zoom), (int)MathF.Ceiling(Zoom), Color.Red);
    }
    static readonly List<Vector2> triangle = new()
    {
        new Vector2(0, -10),
        new Vector2(10, 10),
        new Vector2(-10, 10),
    };
    static readonly List<Vector2> shape = new()
    {
        new Vector2(-4, -4),
        new Vector2(4, -4),
        new Vector2(4, 4),
        new Vector2(-4, 4),
    };
    public static void DrawWorm(Entity entity)
    {
        DrawWireFrameModel(shape, entity.Position.X, entity.Position.Y, entity.Rotation, 1.0f, Color.Red);
    }
    public static void DrawMissile(Entity entity)
    {
        DrawWireFrameModel(triangle, entity.Position.X, entity.Position.Y, entity.Rotation, 1.0f, Color.Red);
    }
    public static void Draw(this Entity entity)
    {
        switch ((EntityModelType)entity.ModelData)
        {
            case EntityModelType.None:
                DrawScaledPixel((int)entity.Position.X, (int)entity.Position.Y, Color.Magenta);
                break;
            case EntityModelType.Worm:
                DrawWorm(entity);
                break;
            case EntityModelType.Missile:
                DrawMissile(entity);
                break;
            case EntityModelType.Dummy:
                DrawDummy(entity);
                //DrawScaledPixel((int)entity.Position.X, (int)entity.Position.Y, Color.Magenta);

                break;
            default:
                throw new Exception($"Unknow model data {entity.ModelData}");
        }
    }
}