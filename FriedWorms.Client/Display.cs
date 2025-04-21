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
    static void Display()
    {
        int screenWidthInTiles = (int)MathF.Ceiling(TARGET_WIDTH / Zoom);
        int screenHeightInTiles = (int)MathF.Ceiling(TARGET_HEIGHT / Zoom);


        for (int x = 0; x < screenWidthInTiles; x++)
        {
            int mapX = x + (int)CameraPosX;
            if (mapX >= MapWidth) continue;

            for (int y = 0; y < screenHeightInTiles; y++)
            {
                int mapY = y + (int)CameraPosY;
                if (mapY >= MapHeight) continue;

                int index = mapY * MapWidth + mapX;

                switch (Map[index])
                {
                    case 0:
                        DrawScaledPixel(x, y, Color.SkyBlue);
                        break;
                    case 1:
                        DrawScaledPixel(x, y, Color.DarkGreen);
                        break;
                }
            }
        }

        foreach (var entity in Entities)
        {
            entity.Draw();
        }

    }
    static void DrawScaledPixel(int x, int y, Color color)
    {
        int px = (int)(x * Zoom);
        int py = (int)(y * Zoom);
        int size = (int)MathF.Ceiling(Zoom);

        for (int dx = 0; dx < size; dx++)
        {
            for (int dy = 0; dy < size; dy++)
            {
                DrawPixel(px + dx, py + dy, color);
            }
        }
    }
    static void DrawWireFrameModelStaticSize(
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

        List<Vector2> transformed = new(vertexCount);

        for (int i = 0; i < vertexCount; i++)
        {
            var pt = modelCoords[i];

            // Rotate
            float rotatedX = pt.X * MathF.Cos(rotation) - pt.Y * MathF.Sin(rotation);
            float rotatedY = pt.X * MathF.Sin(rotation) + pt.Y * MathF.Cos(rotation);

            // Scale (local shape size only, not zoom!)
            rotatedX *= scale;
            rotatedY *= scale;

            // Offset from world position
            float worldX = rotatedX + x;
            float worldY = rotatedY + y;

            // Convert world → screen (zoom affects position, not size)
            float screenX = (worldX - CameraPosX) * Zoom;
            float screenY = (worldY - CameraPosY) * Zoom;

            transformed.Add(new Vector2(screenX, screenY));
        }

        for (int i = 0; i < vertexCount; i++)
        {
            Vector2 p1 = transformed[i];
            Vector2 p2 = transformed[(i + 1) % vertexCount];

            // Screen pixels — don’t scale line length with zoom
            DrawLine((int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y, color);
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
            float screenX = (worldX - CameraPosX) * Zoom;
            float screenY = (worldY - CameraPosY) * Zoom;

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
