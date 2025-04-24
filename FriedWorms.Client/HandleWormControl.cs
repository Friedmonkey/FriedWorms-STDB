using Raylib_cs;
using SpacetimeDB.Types;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    static void HandleWormControl(float elapsedTime)
    {
        if (!UserHasControl || ControlWorm == null) //user doest have control so we skip all this
            return;

        if (IsKeyPressed(KeyboardKey.Space)) //jump left
        {
            ControlWorm.Velocity.X = 6.0f * MathF.Cos(ControlWorm.Rotation);
            ControlWorm.Velocity.Y = -12.0f * MathF.Sin(ControlWorm.Rotation);
            ControlWorm.Stable = false;
        }
        //if (IsKeyPressed(KeyboardKey.D)) //jump right
        //{
        //    ControlWorm.Velocity.X = +4.0f;
        //    ControlWorm.Velocity.Y = -8.0f;   
        //    ControlWorm.Stable = false;
        //}


        if (IsKeyDown(KeyboardKey.Q)) //aim left
        {
            ControlWorm.Rotation += 1.5f * elapsedTime;
            if (ControlWorm.Rotation > MathF.PI) ControlWorm.Rotation -= MathF.PI * 2.0f;
        }
        if (IsKeyDown(KeyboardKey.E)) //aim right
        {
            ControlWorm.Rotation -= 1.5f * elapsedTime;
            if (ControlWorm.Rotation < -MathF.PI) ControlWorm.Rotation += MathF.PI * 2.0f;
        }
    }
}
