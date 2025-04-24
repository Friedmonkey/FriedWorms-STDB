using Raylib_cs;
using SpacetimeDB.Types;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    static void HandleWormControl(float elapsedTime)
    {
        if (!UserHasControl || ObjectUnderControl == null) //user doest have control so we skip all this
            return;

        if (IsKeyPressed(KeyboardKey.A)) //jump left
        {
            ObjectUnderControl.Velocity.X = -4.0f;
            ObjectUnderControl.Velocity.Y = -8.0f;
            ObjectUnderControl.Stable = false;
        }
        if (IsKeyPressed(KeyboardKey.D)) //jump right
        {
            ObjectUnderControl.Velocity.X = +4.0f;
            ObjectUnderControl.Velocity.Y = -8.0f;
            ObjectUnderControl.Stable = false;
        }


    }
}
