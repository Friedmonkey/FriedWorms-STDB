using Raylib_cs;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    static void RenderUI()
    {
        //blah blah blah
        var width = (TARGET_WIDTH * UiScale);
        var height = (TARGET_HEIGHT * UiScale);

        var startLeft = (height / 2)-100;
        for (int i = 1; i < 11; i++)
        {
            DrawCircle(15, startLeft + i*20, 10f, Color.DarkPurple);
        }
    }
}
