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
    static EntityModelType weaponType = EntityModelType.Missile;
    static void ShootWeapon()
    {
        PlayerActionComplete = true;

        //origin
        var ox = ControlWorm.Position.X;
        var oy = ControlWorm.Position.Y;

        //direction
        var dx = MathF.Cos(ControlWorm.ShootingAngle);
        var dy = MathF.Sin(ControlWorm.ShootingAngle);


        //roughly the position of the cursor
        var cursorPosX = (ox + 8 * dx);
        var cursorPosY = (oy + 8 * dy);

        var projectile = SpawnEntity(new DbVector2(cursorPosX, cursorPosY), weaponType);
        var soundIdx = rockets.Play();
        projectile.SoundIdx = soundIdx;
        projectile.Velocity.X = dx * 40.0f * EnergyLevel;
        projectile.Velocity.Y = dy * 40.0f * EnergyLevel;
        CameraTracking = projectile;
        Entities.Add(projectile);
    }
    static void HandleWormControl(float elapsedTime)
    {
        if (!UserHasControl || ControlWorm == null) //user doest have control so we skip all this
            return;

        if (IsKeyPressed(KeyboardKey.W)) //jump
        {
            ControlWorm.Velocity.X = 6.0f * MathF.Cos(ControlWorm.ShootingAngle);
            ControlWorm.Velocity.Y = 12.0f * MathF.Sin(ControlWorm.ShootingAngle);
            ControlWorm.Stable = false;
        }

        if (IsKeyDown(KeyboardKey.A)) //aim left
        {
            ControlWorm.ShootingAngle -= 1.5f * elapsedTime;
            if (ControlWorm.ShootingAngle < -MathF.PI) ControlWorm.ShootingAngle += MathF.PI * 2.0f;
        }
        if (IsKeyDown(KeyboardKey.D)) //aim right
        {
            ControlWorm.ShootingAngle += 1.5f * elapsedTime;
            if (ControlWorm.ShootingAngle > MathF.PI) ControlWorm.ShootingAngle -= MathF.PI * 2.0f;
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
            ShootWeapon();
            EnergyRising = false;
            FireWeapon = false;
            EnergyLevel = 0.0f;
        }
    }
}
