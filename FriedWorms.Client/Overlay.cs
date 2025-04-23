using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    static void Overlay()
    {
        foreach (var entity in Entities.Where(e => e.ModelData == (uint)EntityModelType.Dummy))
        {
            float angle = MathF.Atan2(entity.Velocity.Y, entity.Velocity.X);

            var elapsedTime = GetFrameTime();

            float potentialX = entity.Position.X + entity.Velocity.X * elapsedTime;
            float potentialY = entity.Position.Y + entity.Velocity.Y * elapsedTime;

            for (float radius = (angle - MathF.PI / 2.0f); radius < angle + MathF.PI / 2; radius += MathF.PI / 10.0f)
            {
                float testPosX = (entity.Radius) * MathF.Cos(radius) + potentialX;
                float testPosY = (entity.Radius) * MathF.Sin(radius) + potentialY;

                // Constrain to test within map boundary
                if (testPosX >= MapWidth) testPosX = MapWidth - 1;
                if (testPosY >= MapHeight) testPosY = MapHeight - 1;
                if (testPosX < 0) testPosX = 0;
                if (testPosY < 0) testPosY = 0;

                int newX = (int)Math.Round((testPosX * OverlayScale) - (CameraPosX * OverlayScale));
                int newY = (int)Math.Round((testPosY * OverlayScale) - (CameraPosY * OverlayScale));

                DrawPixel(newX, newY, Color.Red);
            }
        }
    }
}
