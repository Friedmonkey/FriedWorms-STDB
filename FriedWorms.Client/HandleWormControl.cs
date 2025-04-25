using Raylib_cs;
using SpacetimeDB.Types;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    static bool FireWeapon;
    static bool EnergyRising;
    static float EnergyLevel;
    static void HandleWormControl(float elapsedTime)
    {
        if (!UserHasControl || ControlWorm == null) //user doest have control so we skip all this
            return;

        if (IsKeyPressed(KeyboardKey.W)) //jump
        {
            ControlWorm.Velocity.X = 6.0f * MathF.Cos(ControlWorm.Rotation);
            ControlWorm.Velocity.Y = -12.0f * MathF.Sin(ControlWorm.Rotation);
            ControlWorm.Stable = false;
        }

        if (IsKeyDown(KeyboardKey.A)) //aim left
        {
            ControlWorm.Rotation += 1.5f * elapsedTime;
            if (ControlWorm.Rotation > MathF.PI) ControlWorm.Rotation -= MathF.PI * 2.0f;
        }
        if (IsKeyDown(KeyboardKey.D)) //aim right
        {
            ControlWorm.Rotation -= 1.5f * elapsedTime;
            if (ControlWorm.Rotation < -MathF.PI) ControlWorm.Rotation += MathF.PI * 2.0f;
        }

        if (IsKeyPressed(KeyboardKey.Space))
        { 
            EnergyRising = true;
            FireWeapon = false;
            EnergyLevel = 0.0f;
        }
        if (IsKeyDown(KeyboardKey.Space))
        {
            if (EnergyRising)
            {
                EnergyLevel += 0.75f * elapsedTime;
                if (EnergyLevel >= 1.0f)
                {
                    EnergyLevel = 1.0f;
                    FireWeapon = true;
                }
            }
        }
        if (IsKeyReleased(KeyboardKey.Space))
        {
            if (EnergyRising)
            {
                FireWeapon = true;
            }
            EnergyRising = false;
        }

        if (FireWeapon)
        {
            //origin
            var ox = ControlWorm.Position.X;
            var oy = ControlWorm.Position.Y;

            //direction
            var dx = MathF.Cos(ControlWorm.Rotation);
            var dy = MathF.Sin(ControlWorm.Rotation);

            var missile = CreateEntityMissile(ControlWorm.Position);
            missile.Velocity.X = dx * 40.0f * EnergyLevel;
            missile.Velocity.Y = -(dy * 40.0f * EnergyLevel);
            Entities.Add(missile);

            EnergyRising = true;
            FireWeapon = false;
            EnergyLevel = 0.0f;
        }
    }
}
