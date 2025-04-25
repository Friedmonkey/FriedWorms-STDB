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

    Rectangle frameRec;
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

        frameRec = new ( 0, 0, frameWidth / Program.Zoom, frameHeight / Program.Zoom);
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

        frameRec.X = ((frameWidth / Program.Zoom) * (currentFrame * Program.Zoom));
        frameRec.Y = ((frameHeight / Program.Zoom) * (currentLine * Program.Zoom));
    }
    public void Draw(int x, int y, Color color)
    {
        Raylib.DrawTextureRec(texture, frameRec, new( (float)(x - Program.CameraPosX), (float)(y - Program.CameraPosY) ), color);
    }
}
