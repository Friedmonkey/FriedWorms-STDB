using Raylib_cs;
using SpacetimeDB.Types;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    static void HandleMouseClick()
    {
        if (UIClick())
            return;
        if (!TryGetMouseWorldPos(out var world))
            return;


        if (IsMouseButtonPressed(MouseButton.Right))
        {
            Entities.Add(CreateEntityMissile(new(world.X, world.Y)));
        }
        if (IsMouseButtonPressed(MouseButton.Left))
        {
            var worm = CreateEntityWorm(new(world.X, world.Y));
            if (ControlWorm != null)
                ControlWorm.ShootingAngle = float.NegativeZero;

            ControlWorm = worm;
            ControlWorm.ShootingAngle = 1;
            CameraTracking = worm;
            Entities.Add(worm);
        }
        if (IsMouseButtonPressed(MouseButton.Middle))
        {
            Entities.Add(CreateEntityDummy(new(world.X, world.Y)));
        }
    }
    static void HandleUserInput(float elapsedTime)
    {
        if (IsKeyPressed(KeyboardKey.M))
        { 
            CreateMap();
            Entities.Clear();
        }


        if (IsMouseButtonPressed(MouseButton.Left) || IsMouseButtonPressed(MouseButton.Middle) || IsMouseButtonPressed(MouseButton.Right))
        {
            HandleMouseClick();    
        }
        if (IsKeyPressed(KeyboardKey.G) && TryGetMouseWorldPos(out var world))
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
