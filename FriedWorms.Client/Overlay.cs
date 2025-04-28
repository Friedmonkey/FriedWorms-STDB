using Raylib_cs;
using SpacetimeDB.Types;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    static void Overlay()
    {
        if (EnergyRising && ControlWorm != null && !ControlWorm.Dead)
        {
            RenderEnergyBar(ControlWorm);
        }
        //render health bars
        foreach (var entity in Entities)
        {
            if (entity.MaxHealth != 0)
                RenderHealthBar(entity);

            if (entity.ShootingAngle != float.NegativeZero)
            { 
                int centerX = (int)((entity.Position.X * OverlayScale) + 35.0f * MathF.Cos(entity.ShootingAngle) - (CameraPosX * OverlayScale));
                int centerY = (int)((entity.Position.Y * OverlayScale) + 35.0f * MathF.Sin(entity.ShootingAngle) - (CameraPosY * OverlayScale));

                System.Numerics.Vector2 center = new(centerX, centerY);
                System.Numerics.Vector2 scaleX = new (8, 0);
                System.Numerics.Vector2 scaleY = new (0, 8);
                DrawLineEx(center-scaleX, center+scaleX, 4, Color.Black);
                DrawLineEx(center-scaleY, center+scaleY, 4, Color.Black);
            }
            //if (entity.ModelData != (int)EntityModelType.Debris)
            //    DisplayRotation(entity);
        }

    }
    static void DisplayRotation(Entity entity)
    {
        var rotation = MathF.Atan2(entity.Velocity.Y, entity.Velocity.X);
        if (entity.ModelData is 1 or 5 && entity.Stable)
        {
            rotation = MathF.Abs(rotation);
            rotation = Math.Clamp(rotation, 3 - 0.2f, 3 + 0.2f);
        }

        float textX = (entity.Position.X * OverlayScale) - (CameraPosX * OverlayScale);
        float textY = (entity.Position.Y * OverlayScale) - (CameraPosY * OverlayScale);
        DrawText(rotation.ToString(), (int)textX, (int)textY, 5, Color.Black);
    }
    static void RenderEnergyBar(Entity entity)
    {
        //render energy bar
        int energyPixels = (int)MathF.Round(EnergyLevel * 50);

        int barPixelsX = 5;
        int barPixelsY = 50;

        float barX = (entity.Position.X * OverlayScale) - (CameraPosX * OverlayScale);
        float barY = (entity.Position.Y * OverlayScale) - (CameraPosY * OverlayScale);

        for (int i = 0; i < barPixelsY; i++)
        {
            Color color = (i > energyPixels) ? Color.DarkPurple : Color.Blue;

            int newX = (int)Math.Round(barX) - 50;
            int newY = (int)Math.Round(barY) - i;

            DrawLine(newX, newY, newX + barPixelsX, newY, color);
        }
    }
    static void RenderHealthBar(Entity entity)
    {
        //render health bar
        int healthPixels = (int)MathF.Round(entity.Health / 2);

        int barPixelsX = (int)MathF.Round(entity.MaxHealth / 2);
        int barPixelsY = 5;

        float barX = (entity.Position.X * OverlayScale) - (CameraPosX * OverlayScale);
        float barY = (entity.Position.Y * OverlayScale) - (CameraPosY * OverlayScale);

        for (int i = 0; i < barPixelsX; i++)
        {
            Color color = (i > healthPixels) ? Color.Red : Color.Green;

            int newX = (int)Math.Floor(barX - (healthPixels / 2) + i);
            int newY = (int)Math.Floor(barY) - 25;

            DrawLine(newX, newY, newX, newY + barPixelsY, color);
        }
    }
}
