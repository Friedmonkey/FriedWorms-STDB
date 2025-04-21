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
                        DrawScaledPixel(x, y, Zoom, Color.SkyBlue);
                        break;
                    case 1:
                        DrawScaledPixel(x, y, Zoom, Color.DarkGreen);
                        break;
                }
            }
        }


    }
    static void DrawScaledPixel(int x, int y, float scale, Color color)
    {
        int px = (int)(x * scale);
        int py = (int)(y * scale);
        int size = (int)MathF.Ceiling(scale);

        for (int dx = 0; dx < size; dx++)
        {
            for (int dy = 0; dy < size; dy++)
            {
                DrawPixel(px + dx, py + dy, color);
            }
        }
    }

}
