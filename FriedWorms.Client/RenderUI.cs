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
    static void RenderUI()
    {
        //blah blah blah
        var width = (TARGET_WIDTH * UiScale);
        var height = (TARGET_HEIGHT * UiScale);

        DrawCircle(width/2, height/2, 10f, Color.DarkPurple);
    }
}
