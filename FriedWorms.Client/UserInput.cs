using Raylib_cs;
using SpacetimeDB.Types;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    static void HandleUserInput(float elapsedTime)
    {
        if (IsKeyPressed(KeyboardKey.M))
            CreateMap();

        if (IsMouseButtonPressed(MouseButton.Right) && TryGetMouseWorldPos(out var world))
        {
            Entities.Add(CreateEntityMissile(new(world.X, world.Y)));
        }
        if (IsMouseButtonPressed(MouseButton.Left) && TryGetMouseWorldPos(out world))
        {
            var worm = CreateEntityWorm(new(world.X, world.Y));
            if (ControlWorm != null)
                ControlWorm.Rotation = float.NegativeZero;

            ControlWorm = worm;
            ControlWorm.Rotation = 1;
            //CameraTracking = worm;
            Entities.Add(worm);
        }
        if (IsMouseButtonPressed(MouseButton.Middle) && TryGetMouseWorldPos(out world))
        {
            Entities.Add(CreateEntityDummy(new(world.X, world.Y)));
        }
        if (IsKeyPressed(KeyboardKey.G) && TryGetMouseWorldPos(out world))
        {
            Entities.Add(CreateEntityGranade(new(world.X, world.Y)));
        }

        if (IsKeyDown(KeyboardKey.Equal) || IsKeyDown(KeyboardKey.Minus))
        {
            float oldZoom = Zoom;

            Vector2 center = new(TARGET_WIDTH / 2f, TARGET_HEIGHT / 2f);
            Vector2 worldCenter = new Vector2(CameraPosX, CameraPosY) + center / oldZoom;

            if (IsKeyDown(KeyboardKey.Equal))
                Zoom = Math.Clamp(Zoom + 0.1f, 1.0f, MaxZoom);
            else
                Zoom = Math.Clamp(Zoom - 0.1f, 1.0f, MaxZoom);

            if (Zoom != oldZoom)
            {
                Vector2 newCam = worldCenter - center / Zoom;
                CameraPosX = newCam.X;
                CameraPosY = newCam.Y;
            }
        }

        float mapScrollSpeed = 300.0f / Zoom;
        if (mapScrollSpeed < 30)
            mapScrollSpeed = 30;

        if (IsKeyDown(KeyboardKey.Up))
            CameraPosY -= mapScrollSpeed * elapsedTime;
        if (IsKeyDown(KeyboardKey.Down))
            CameraPosY += mapScrollSpeed * elapsedTime;
        if (IsKeyDown(KeyboardKey.Left))
            CameraPosX -= mapScrollSpeed * elapsedTime;
        if (IsKeyDown(KeyboardKey.Right))
            CameraPosX += mapScrollSpeed * elapsedTime;

        if (IsKeyPressed(KeyboardKey.P))
            PhysicsPaused = !PhysicsPaused;
    }
}
