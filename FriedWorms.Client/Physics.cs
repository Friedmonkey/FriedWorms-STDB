using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedWorms.Client;

public partial class Program
{
    static void HandlePhysics(float elapsedTime)
    {
        foreach (var entity in Entities)
        {
            //add gravity
            entity.Acceleration.Y += 2.0f;

            //update velocity
            entity.Velocity.X += entity.Acceleration.X * elapsedTime;
            entity.Velocity.Y += entity.Acceleration.Y * elapsedTime;

            //update position
            float potentialX = entity.Position.X + entity.Velocity.X * elapsedTime;
            float potentialY = entity.Position.Y + entity.Velocity.Y * elapsedTime;

            //reset accel
            entity.Acceleration = new(0, 0);
            entity.Stable = false;

            entity.Position = new(potentialX, potentialY);
        }
    }
    static void ApplyGravity()
    { 
        
    }

}
