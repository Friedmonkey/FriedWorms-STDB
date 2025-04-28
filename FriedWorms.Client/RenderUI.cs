using Raylib_cs;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    const int BUTTON_SCALE = 25;
    const int BUTTON_PADDING = 10;

    static List<UIButton> buttons;
    static int startLeft = 0;
    static void LoadUI()
    {
        buttons = new List<UIButton>();
        var height = (TARGET_HEIGHT * UiScale);
        startLeft = (height / 2);

        AddButton();
        AddButton();
        AddButton();
        AddButton();
        AddButton();
    }
    static bool UIClick()
    {
        return false;
    }
    static void RenderUI()
    {
        foreach (var button in buttons) 
        {
            var offset = (buttons.Count * (BUTTON_SCALE * 2 + BUTTON_PADDING) / 2);
            DrawCircle(button.centerX, button.centerY-offset, BUTTON_SCALE, button.color);
        }
    }
    static void AddButton()
    {
        startLeft += (BUTTON_SCALE * 2 + BUTTON_PADDING);
        buttons.Add(new UIButton() 
        {
            centerX = 40,
            centerY = startLeft,
            color = Color.DarkPurple,
        });
    }
    public class UIButton
    {
        public int centerX;
        public int centerY;
        public Color color;
    }
}
