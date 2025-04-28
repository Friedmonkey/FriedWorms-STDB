using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FriedWorms.Client;

public class FriedAnimatedTexure
{
    Texture2D texture;
    int framesPerLine;
    int lines;

    // Init variables for animation
    float frameWidth;
    float frameHeight;
    int currentFrame;
    int currentLine;

    Rectangle sourceRec;
    Rectangle destRec;
    Vector2 position;

    int framesCounter;
    public FriedAnimatedTexure(Texture2D texture, int framesPerLine, int lines)
    {
        this.framesPerLine = framesPerLine;
        this.lines = lines;
        this.texture = texture;

        // Init variables for animation
        frameWidth = (float)(texture.Width / framesPerLine);    // Sprite one frame rectangle width
        frameHeight = (float)(texture.Height / lines);          // Sprite one frame rectangle height
        currentFrame = 0;
        currentLine = 0;

        sourceRec = new ( 0, 0, frameWidth, frameHeight);
        destRec = new ( 0, 0, frameWidth, frameHeight);
        position = new ( 0.0f, 0.0f );

        framesCounter = 0;
    }
    public void Update()
    {
        // Compute explosion animation frames
        framesCounter++;

        if (framesCounter > 2)
        {
            currentFrame++;

            if (currentFrame >= framesPerLine)
            {
                currentFrame = 0;
                currentLine++;

                if (currentLine >= lines)
                {
                    currentLine = 0;
                }
            }

            framesCounter = 0;
        }


        //float zoom = (Raylib.GetScreenWidth() / (float)Program.TARGET_WIDTH);

        //// Calculate new width and height of the scaled texture
        //int scaledWidth = (int)(Program.TARGET_WIDTH * zoom);
        //int scaledHeight = (int)(Program.TARGET_HEIGHT * zoom);

        //// Offset so the zoom centers in the middle of the screen
        //int offsetX = (Raylib.GetScreenWidth() - scaledWidth) / 2;
        //int offsetY = (Raylib.GetScreenHeight() - scaledHeight) / 2;

        //scaledWidth = (int)(scaledWidth * Program.Zoom);
        //scaledHeight = (int)(scaledHeight * Program.Zoom);

        //sourceRec.X = ((frameWidth / Program.Zoom) * (currentFrame * Program.Zoom));
        //sourceRec.Y = ((frameHeight / Program.Zoom) * (currentLine * Program.Zoom));
        //sourceRec.Width = frameWidth / Program.Zoom;
        //sourceRec.Height = frameHeight / Program.Zoom;

        //destRec.Width = (Program.MapWidth);/// Program.Zoom) / Program.BackgroundScale;
        //destRec.Height = (Program.MapHeight);/// Program.Zoom) / Program.BackgroundScale + 150;
        //destRec.X = Program.CameraPosX;
        //destRec.Y = Program.CameraPosY;

        sourceRec.X = (frameWidth * currentFrame);
        sourceRec.Y = (frameHeight * currentLine);
        sourceRec.Width = frameWidth;
        sourceRec.Height = frameHeight;

        destRec.X = 0;
        destRec.Y = 0;
        destRec.Width = frameWidth;
        destRec.Height = frameHeight;

    }
    public void Draw(int x, int y, Color color)
    {
        //Raylib.DrawTexture(texture, x, y, color);
        Raylib.DrawTextureRec(texture, sourceRec, new( (float)(x), (float)(y) ), color);
        //Raylib.DrawTexturePro(texture, sourceRec, destRec, new((float)(x), (float)(y)), 0f, color);
    }
}
