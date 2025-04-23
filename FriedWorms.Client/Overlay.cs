using Raylib_cs;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    static void Overlay()
    {
        //render health bars
        foreach (var entity in Entities.Where(e => e.MaxHealth != 0))
        {
            int healthPixels = (int)MathF.Round(entity.Health/2);

            int barPixelsX = (int)MathF.Round(entity.MaxHealth/2);
            int barPixelsY = 5;

            float barX = (entity.Position.X * OverlayScale) - (CameraPosX * OverlayScale);
            float barY = (entity.Position.Y * OverlayScale) - (CameraPosY * OverlayScale);



            for (int i = 0; i < barPixelsX; i++)
            {
                Color color = (i > healthPixels) ? Color.Red : Color.Green;

                int newX = (int)Math.Round(barX-(healthPixels/2)+i);
                int newY = (int)Math.Round(barY) - 25;

                DrawLine(newX, newY, newX, newY + barPixelsY, color);
            }



            //float angle = MathF.Atan2(entity.Velocity.Y, entity.Velocity.X);

            //var elapsedTime = GetFrameTime();

            //float potentialX = entity.Position.X + entity.Velocity.X * elapsedTime;
            //float potentialY = entity.Position.Y + entity.Velocity.Y * elapsedTime;

            //for (float radius = (angle - MathF.PI / 2.0f); radius < angle + MathF.PI / 2; radius += MathF.PI / 10.0f)
            //{
            //    float testPosX = (entity.Radius) * MathF.Cos(radius) + potentialX;
            //    float testPosY = (entity.Radius) * MathF.Sin(radius) + potentialY;

            //    // Constrain to test within map boundary
            //    if (testPosX >= MapWidth) testPosX = MapWidth - 1;
            //    if (testPosY >= MapHeight) testPosY = MapHeight - 1;
            //    if (testPosX < 0) testPosX = 0;
            //    if (testPosY < 0) testPosY = 0;

            //    int newX = (int)Math.Round((testPosX * OverlayScale) - (CameraPosX * OverlayScale));
            //    int newY = (int)Math.Round((testPosY * OverlayScale) - (CameraPosY * OverlayScale));

            //    DrawPixel(newX, newY, Color.Red);
            //}
        }
    }
}
