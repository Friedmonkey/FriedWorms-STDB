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


            float angle = MathF.Atan2(entity.Velocity.Y, entity.Velocity.X);
            float responseX = 0F;
            float responseY = 0F;
            bool collison = false;

            for (float radius = (angle - MathF.PI / 2.0f); radius < angle + MathF.PI/2; radius += MathF.PI / 8.0f)
            {
                float testPosX = (entity.Radius) * MathF.Cos(radius) + potentialX;
                float testPosY = (entity.Radius) * MathF.Sin(radius) + potentialY;

                // Constrain to test within map boundary
                if (testPosX >= MapWidth) testPosX = MapWidth - 1;
                if (testPosY >= MapHeight) testPosY = MapHeight - 1;
                if (testPosX < 0) testPosX = 0;
                if (testPosY < 0) testPosY = 0;

                if (Map[(int)testPosY * MapWidth + (int)testPosX] != 0) //if sky
                {
                    responseX += potentialX - testPosX;
                    responseY += potentialY - testPosY;
                    collison = true;
                }


                float magVelocity = MathF.Sqrt(entity.Velocity.X * entity.Velocity.X + entity.Velocity.Y * entity.Velocity.Y);
                float magResponse = MathF.Sqrt(responseX*responseX + responseY*responseY);

                if (collison)
                {
                    entity.Stable = true;

                    float dot = entity.Velocity.X * (responseX / magResponse) + entity.Velocity.Y * (responseY / magResponse);
                    entity.Velocity.X = (-2.0f * dot * (responseX / magResponse) + entity.Velocity.X);
                    entity.Velocity.Y = (-2.0f * dot * (responseY / magResponse) + entity.Velocity.Y);

                }
                else
                {   
                    entity.Position = new(potentialX, potentialY);
                }

                if (magVelocity < 0.1f) entity.Stable = true;
            }

        }
    }
    static void ApplyGravity()
    { 
        
    }

}
