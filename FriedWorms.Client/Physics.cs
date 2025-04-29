using SpacetimeDB.Types;

namespace FriedWorms.Client;

public partial class Program
{
    static void HandlePhysics(float elapsedTime)
    {
        for (int i = 0; i < Entities.Count; i++)
        {
            Entity? entity = Entities[i];
            entity.OnTick(elapsedTime);

            if (entity.DeathTimer != float.PositiveInfinity)
                entity.DeathTimer -= 1 * elapsedTime;

            if (entity.DeathTimer <= 0)
                entity.Dead = true;

            //kill offscreen entities (except tracking entities, so that missiles can go offscreen for a bit and still hit their target)
            if (entity.Position.Y <= 0 && entity.Id != CameraTracking?.Id) 
                entity.Dead = true;
            if (entity.Position.Y >= MapHeight)
                entity.Dead = true;

            //add gravity
            entity.Acceleration.Y += 2.0f + entity.ExtraGravityForce;

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

            for (float radius = (angle - MathF.PI / 2.0f); radius < angle + MathF.PI / 2; radius += MathF.PI / 10.0f)
            {
                float testPosX = (entity.Radius) * MathF.Cos(radius) + potentialX;
                float testPosY = (entity.Radius) * MathF.Sin(radius) + potentialY;

                // Constrain to test within map boundary
                if (testPosX >= MapWidth) testPosX = MapWidth - 1;
                if (testPosY >= MapHeight) testPosY = MapHeight - 1;
                if (testPosX < 0) testPosX = 0;
                if (testPosY < 0) testPosY = 0;

                if (!(Map[(int)testPosY * MapWidth + (int)testPosX] is (int)MapColor.Skyblue or (int)MapColor.Cloud)) //if not sky
                {
                    responseX += potentialX - testPosX;
                    responseY += potentialY - testPosY;
                    collison = true;
                }
            }

            float magVelocity = MathF.Sqrt(entity.Velocity.X * entity.Velocity.X + entity.Velocity.Y * entity.Velocity.Y);
            float magResponse = MathF.Sqrt(responseX*responseX + responseY*responseY);

            if (collison)
            {
                entity.Stable = true;

                float dot = entity.Velocity.X * (responseX / magResponse) + entity.Velocity.Y * (responseY / magResponse);
                entity.Velocity.X = entity.Friction * (-2.0f * dot * (responseX / magResponse) + entity.Velocity.X);
                entity.Velocity.Y = entity.Friction * (-2.0f * dot * (responseY / magResponse) + entity.Velocity.Y);

                if (entity.MaxBounceCount > 0)
                {
                    entity.MaxBounceCount--;
                    entity.Dead = (entity.MaxBounceCount == 0);
                }
            }
            else
            {   
                entity.Position = new(potentialX, potentialY);
            }

            if (magVelocity < 0.1f) entity.Stable = true;

            if (entity.Dead)
                entity.OnDeath();
        }

        //remove dead entities
        Entities.RemoveAll(x => x.Dead);
    }
}
