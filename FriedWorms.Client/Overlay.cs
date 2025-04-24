using Raylib_cs;
using SpacetimeDB.Types;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    static void Overlay()
    {
        //render health bars
        foreach (var entity in Entities)
        {
            if (entity.MaxHealth != 0)
                RenderHealthBar(entity);

            if (entity.ModelData != (int)EntityModelType.Debris)
                DisplayRotation(entity);
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

            int newX = (int)Math.Round(barX - (healthPixels / 2) + i);
            int newY = (int)Math.Round(barY) - 25;

            DrawLine(newX, newY, newX, newY + barPixelsY, color);
        }
    }
}
