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
        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                int mapX = x + (int)CameraPosX;
                int mapY = y + (int)CameraPosY;

                if (!(mapX >= 0 && mapX < MapWidth && mapY >= 0 && mapY < MapHeight))
                    continue;

                int index = mapY * MapWidth + mapX;
                switch (Map[index])
                {
                    case 0:
                        DrawPixel(x, y, Color.SkyBlue);
                        break;
                    case 1:
                        DrawPixel(x, y, Color.DarkGreen);
                        break;
                }
            }
        }
    }
}
