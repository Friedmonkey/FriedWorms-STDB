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
        frameWidth = (float)(texture.Width / framesPerLine);   // Sprite one frame rectangle width
        frameHeight = (float)(texture.Height / lines);           // Sprite one frame rectangle height
        currentFrame = 0;
        currentLine = 0;

        frameRec = new ( 0, 0, frameWidth, frameHeight );
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

        frameRec.X = frameWidth * currentFrame;
        frameRec.Y = frameHeight * currentLine;
    }
    public void Draw(int x, int y, Color color)
    {
        Raylib.DrawTextureRec(texture, frameRec, new( (float)(x), (float)(y) ), color);
    }
}
