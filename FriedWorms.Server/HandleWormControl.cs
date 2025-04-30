using static Module;

namespace SpacetimeDB;

public static partial class Module
{
    static bool FireWeapon;
    static bool EnergyRising;
    static float EnergyLevel;
    static EntityModelType weaponType = EntityModelType.Missile;
    [Reducer]
    public static void ShootWeapon(ReducerContext ctx, uint wormId)
    {
        //PlayerActionComplete = true;

        ////origin
        //var ox = ControlWorm.Position.X;
        //var oy = ControlWorm.Position.Y;

        ////direction
        //var dx = MathF.Cos(ControlWorm.ShootingAngle);
        //var dy = MathF.Sin(ControlWorm.ShootingAngle);


        ////roughly the position of the cursor
        //var cursorPosX = (ox + 8 * dx);
        //var cursorPosY = (oy + 8 * dy);

        //var projectile = CreateEntity(new DbVector2(cursorPosX, cursorPosY), weaponType);
        //projectile.Velocity.X = dx * 40.0f * EnergyLevel;
        //projectile.Velocity.Y = dy * 40.0f * EnergyLevel;
        //CameraTracking = projectile;
        //Entities.Add(projectile);
    }
    [Reducer]
    public static void SetshootingAngle(ReducerContext ctx, float newAngle)
    {
        var config = ctx.Db.Config.Id.Find(0) ?? throw new Exception("no config");

        var controlWorm = ctx.Db.Entities.Id.Find(config.ControlWormId) ?? throw new Exception("controlworm not found");
        //if (controlWorm.PlayerId != ctx.Db.Players)
        controlWorm.ShootingAngle = newAngle;
    }
    [Reducer]
    public static void Jump(ReducerContext ctx, float angle)
    {
        var config = ctx.Db.Config.Id.Find(0) ?? throw new Exception("no config");

        var controlWorm = ctx.Db.Entities.Id.Find(config.ControlWormId) ?? throw new Exception("controlworm not found");
        //if (controlWorm.PlayerId != ctx.Db.Players)
        controlWorm.ShootingAngle = angle;

        controlWorm.Velocity.X = 6.0f * MathF.Cos(controlWorm.ShootingAngle);
        controlWorm.Velocity.Y = 12.0f * MathF.Sin(controlWorm.ShootingAngle);
        controlWorm.Stable = false;
    }
   
}
