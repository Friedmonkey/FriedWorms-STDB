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
using System.Runtime.Intrinsics;

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
        DrawWireFrameModel(dummy, entity.Position.X, entity.Position.Y, MathF.Atan2(entity.Velocity.Y, entity.Velocity.X), 1.0f, Color.White);
    }
    public static void DrawWorm(Entity entity)
    {
        DrawWireFrameModel(shape, entity.Position.X, entity.Position.Y, MathF.Atan2(entity.Velocity.Y, entity.Velocity.X    ), 1.0f, Color.Red);
    }
    public static void DrawMissile(Entity entity)
    {
        DrawWireFrameModel(missile, entity.Position.X, entity.Position.Y, MathF.Atan2(entity.Velocity.Y, entity.Velocity.X  ), 1.0f, Color.Red);
    }
    public static void Draw(this Entity entity)
    {
        switch ((EntityModelType)entity.ModelData)
        {
            case EntityModelType.None:
                DrawPixel((int)entity.Position.X, (int)entity.Position.Y, Color.Magenta);
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