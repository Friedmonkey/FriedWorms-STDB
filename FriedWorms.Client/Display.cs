using Raylib_cs;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    static Color GetMapColor(MapColor color)
    { 
        return (color) switch
        {
            MapColor.Skyblue => new Color(0,0,0,0),//Color.SkyBlue,
            MapColor.Grass1 => Color.DarkGreen,
            MapColor.Grass2 => DarkGreen2,
            MapColor.Rock1 => Color.Gray,
            MapColor.Rock2 => Color.DarkGray,
            MapColor.Cloud => Color.White,
            _ => throw new Exception("Unknown color index:" + (int)color)
        };
    }
    static void Display()
    {
        var displayX = TARGET_WIDTH / Zoom;
        var displayY = TARGET_HEIGHT / Zoom;

        for (int x = 0; x < displayX; x++)
        {
            int mapX = x + (int)CameraPosX;
            if (mapX >= MapWidth) continue;

            for (int y = 0; y < displayY; y++)
            {
                int mapY = y + (int)CameraPosY;
                if (mapY >= MapHeight) continue;

                int index = mapY * MapWidth + mapX;

                var color = GetMapColor((MapColor)Map[index]);
                DrawPixel(x, y, color);

                if (Map[index] == (int)MapColor.Cloud)
                {
                    //DrawCircle(x, y, 10, color);
                    DrawEllipse(x, y, 15, 6, color);
                }

                    //case 0:
                    //    DrawPixel(x, y, Color.SkyBlue);
                    //    break;
                    //case 1:
                    //        colo
                    //    DrawPixel(x, y, Color.DarkGreen);
                    //    break;
            }
        }

        foreach (var entity in Entities.Where(e => !e.Dead))
        {
            entity.Draw();
        }

    }

    static void DrawWireFrameModel(
    List<Vector2> modelCoords,
    float x, float y,
    float rotation = 0.0f,
    float scale = 1.0f,
    Color color = default)
    {
        if (color.Equals(default(Color)))
            color = Color.White;

        int vertexCount = modelCoords.Count;
        if (vertexCount < 2) return;

        // Transform each point
        List<Vector2> transformed = new(vertexCount);
        for (int i = 0; i < vertexCount; i++)
        {
            var pt = modelCoords[i];

            // Rotate
            float rotatedX = pt.X * MathF.Cos(rotation) - pt.Y * MathF.Sin(rotation);
            float rotatedY = pt.X * MathF.Sin(rotation) + pt.Y * MathF.Cos(rotation);

            // Scale
            rotatedX *= scale;
            rotatedY *= scale;

            // Translate to world position
            float worldX = rotatedX + x;
            float worldY = rotatedY + y;

            // Convert world → screen (Zoom + CameraPos)
            float screenX = (worldX - CameraPosX);
            float screenY = (worldY - CameraPosY);

            transformed.Add(new Vector2(screenX, screenY));
        }

        // Draw lines
        for (int i = 0; i < vertexCount; i++)
        {
            Vector2 p1 = transformed[i];
            Vector2 p2 = transformed[(i + 1) % vertexCount]; // loop back around

            DrawLine((int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y, color);
        }
    }

}
