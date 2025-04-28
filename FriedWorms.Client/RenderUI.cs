using Raylib_cs;
using SpacetimeDB.Types;
using System.Numerics;
using System.Runtime.CompilerServices;
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

        AddButton(EntityModelType.Missile, selected:true);
        AddButton(EntityModelType.Granade);
        AddButton(EntityModelType.Worm);
        AddButton(EntityModelType.Gravestone);
    }
    static bool UIClick()
    {
        // Only care about left-clicks on UI
        if (!TryGetMouseUIPos(out var uiMouse))
            return false;

        DrawCircleV(uiMouse, 20, Color.Red);

        // Compute the same offset you use when drawing
        float offset = (buttons.Count * (BUTTON_SCALE * 2 + BUTTON_PADDING)) / 2f;

        foreach (var button in buttons)
        {
            // Center of this button in UI coords
            var btnCenter = new Vector2(button.centerX,
                                        button.centerY - offset);

            // Quick squared-distance check
            float dx = uiMouse.X - btnCenter.X;
            float dy = uiMouse.Y - btnCenter.Y;
            if (dx * dx + dy * dy <= (BUTTON_SCALE * BUTTON_SCALE)+150)
            {
                if (IsMouseButtonPressed(MouseButton.Left))
                    button.Select();
                return true;
            }
        }

        return false;
    }
    static void RenderUI()
    {
        var offset = (buttons.Count * (BUTTON_SCALE * 2 + BUTTON_PADDING) / 2);
        foreach (var button in buttons)
        {
            int x = button.centerX;
            var y = button.centerY - offset;

            Color outline = button.Selected ? button.selectedOutlineColor : button.outlineColor;
            Color outlineOut = new Color(outline.R, outline.G, outline.B, (byte)0);

            //slight shadow of 20 bigger
            DrawCircleGradient(x, y, BUTTON_SCALE + 20, outline, outlineOut);
        }
        foreach (var button in buttons)
        {
            int x = button.centerX;
            var y = button.centerY - offset;

            Color inner = button.Selected ? button.selectedInnerColor : button.innerColor;
            Color outer = button.Selected ? button.selectedOuterColor : button.outerColor;
            DrawCircleGradient(x, y, BUTTON_SCALE, inner, outer);
            
            Color outline = button.Selected ? button.selectedOutlineColor : button.outlineColor;
            DrawCircleLines(x, y, BUTTON_SCALE, outline);

            if (button.entityModel is null)
                continue;

            button.entityModel.Position.X = button.centerX;
            button.entityModel.Position.Y = button.centerY - offset;
            button.entityModel.Draw(uiscaling:true);
        }
    }
    static void AddButton(EntityModelType model = EntityModelType.Missile, bool selected = false)
    {
        startLeft += (BUTTON_SCALE * 2 + BUTTON_PADDING);
        buttons.Add(new UIButton(selected)
        {
            centerX = 40,
            centerY = startLeft,
            innerColor = Color.DarkBlue,
            outerColor = Color.DarkBlue,
            selectedInnerColor = Color.Blue,
            selectedOuterColor = Color.DarkBlue,

            outlineColor = Color.Black,
            selectedOutlineColor = Color.White,
            entityModel = CreateEntity(new(0,0), model),
            onClick = () => weaponType = model
        });
    }
    public class UIButton
    {
        public UIButton(bool selected) => this.Selected = selected;
        public int centerX;
        public int centerY;
        public Entity? entityModel;
        public Color innerColor;
        public Color outerColor;
        public Color selectedInnerColor;
        public Color selectedOuterColor;

        public Action? onClick;

        public Color outlineColor;
        public Color selectedOutlineColor;
        public bool Selected { get; private set; }
        public void Select()
        {
            buttons.ForEach(b => b.Selected = false);
            this.Selected = true;
            this.onClick?.Invoke();
        }
    }

    static bool TryGetMouseUIPos(out Vector2 uiPos)
    {
        uiPos = Vector2.Zero;
        Vector2 mouse = GetMousePosition();

        int windowWidth = GetScreenWidth();
        int windowHeight = GetScreenHeight();

        // 1) Calculate the base UI zoom (without camera zoom, only UiScale)
        float baseZoom = windowWidth / ((float)TARGET_WIDTH * UiScale);

        // 2) Calculate the scaled size of the UI
        float scaledWidth = TARGET_WIDTH * UiScale * baseZoom;
        float scaledHeight = TARGET_HEIGHT * UiScale * baseZoom;

        // 3) Center the UI on the screen
        float offsetX = (windowWidth - scaledWidth) / 2f;
        float offsetY = (windowHeight - scaledHeight) / 2f;

        // 4) Check if the mouse is inside the UI area
        if (mouse.X < offsetX || mouse.X > offsetX + scaledWidth ||
            mouse.Y < offsetY || mouse.Y > offsetY + scaledHeight)
            return false;

        // 5) Convert mouse position to UI coordinates
        float renderX = (mouse.X - offsetX) / baseZoom;
        float renderY = (mouse.Y - offsetY) / baseZoom;

        uiPos = new Vector2(renderX, renderY);
        return true;
    }

}
