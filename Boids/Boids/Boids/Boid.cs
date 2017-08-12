using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids
{
    public class Boid
    {
        Texture2D image;
        Vector2 velocity;
        Vector2 acceleration;
        Vector2 location;
        float maxSpeed;
        float maxForce;
        float rotation;
        bool isPerch;
        float perchTime;


        public Boid(Texture2D _image, Vector2 _location)
        {
            image = _image;
            location = _location;
            acceleration = Vector2.Zero;
            velocity = new Vector2(((float)(Flock.r.Next(-10, 10)) / (float)10), ((float)(Flock.r.Next(-10, 10)) / (float)10));
            maxForce = 0.05f;
            maxSpeed = 3.0f;
            rotation = calculateRotation(velocity);
            isPerch = false;
            perchTime = 0f;
        }

        private float calculateRotation(Vector2 velocity)
        {
            velocity.Normalize();
            float rotation = (float)Math.Atan2(velocity.Y, velocity.X);

            return rotation;
        }

        private void applyForce(Vector2 force)
        {

            acceleration += force;
        }


        public void Update(GameTime gametime, List<Boid> boids)
        {
            if (!isPerch)
            {

                flock(boids);


                velocity += (acceleration);
                // Limit speed
                velocity = limitVector(velocity, maxSpeed);
                location += (velocity);
                // Reset accelertion to 0 each cycle
                acceleration *= (0);


                border();

                rotation = calculateRotation(velocity);
                
                    
                    perch();
            }
            else
            {

                perchTime += (float)gametime.ElapsedGameTime.TotalMilliseconds;
                if (perchTime > 4000)
                {
                    perchTime = 0f;
                    isPerch = false;
                }


            }
        }

        private void flock(List<Boid> boids)
        {
            Vector2 seperation = seperate(boids);
            Vector2 alignment = align(boids);
            Vector2 cohesion = cohe(boids);

            seperation *= 1.5f;
            alignment *= 1.0f;
            cohesion *= 1.0f;

            applyForce(seperation);
            applyForce(alignment);
            applyForce(cohesion);



            
        }
        private void border()
        {

        
            location.X = (location.X + Game1.graphics.GraphicsDevice.Viewport.Width) % Game1.graphics.GraphicsDevice.Viewport.Width;

            location.Y = (location.Y + Game1.graphics.GraphicsDevice.Viewport.Height) % Game1.graphics.GraphicsDevice.Viewport.Height;
        
        
        }

        private void perch()
        {
            if (location.Y > 27 && location.Y < 30)
            {
                if (Flock.r.Next(0, 10) > 8)
                {
                    isPerch = true;
                    rotation = MathHelper.ToRadians(90);
                }

            }
        }
        private Vector2 seperate(List<Boid> boids)
        {
            float desiredseparation = 25.0f;
            Vector2 steer = new Vector2(0, 0);
            int count = 0;
            // For every boid in the system, check if it's too close
            foreach (Boid other in boids)
            {
                float d = calcDistance(location, other.location);
                // If the distance is greater than 0 and less than an arbitrary amount (0 when you are yourself)
                if ((d > 0) && (d < desiredseparation))
                {
                    // Calculate vector pointing away from neighbor
                    Vector2 diff = (location - other.location);
                    diff.Normalize();
                    diff /= d;        // Weight by distance
                    steer += diff;
                    count++;            // Keep track of how many
                }
            }
            // Average -- divide by how many
            if (count > 0)
            {
                steer /= ((float)count);
            }

            // As long as the vector is greater than 0
            if (calcMag(steer) > 0)
            {
                // Implement Reynolds: Steering = Desired - Velocity
                steer.Normalize();
                steer *= maxSpeed;
                steer -= (velocity);
                steer = limitVector(steer, maxForce);
            }
            return steer;

        }
 
        private Vector2 align(List<Boid> boids)

        {
            float neighbordist = 50;
            Vector2 sum = new Vector2(0, 0);
            int count = 0;
            foreach (Boid other in boids)
            {
                float d = calcDistance(location, other.location);
                if ((d > 0) && (d < neighbordist))
                {
                    sum += other.velocity;
                    count++;
                }
            }
            if (count > 0)
            {
                sum = calcDiv(sum, count);
                sum.Normalize();
                sum *= maxSpeed;
                Vector2 steer = (sum - velocity);
                steer = limitVector(steer, maxForce);
                return steer;
            }
            else
            {
                return new Vector2(0, 0);
            }

        }

        private Vector2 calcDiv(Vector2 vec, int div)
        {
            return vec / div;

        }

        private float calcDistance(Vector2 a, Vector2 b)
        {
            Vector2 c = b - a;
            return (float)Math.Sqrt(((double)(c.X * c.X)) + ((double)(c.Y * c.Y)));


        }

        private Vector2 cohe(List<Boid> boids)
        {

            float neighbordist = 75;
            Vector2 sum = new Vector2(0, 0);   // Start with empty vector to accumulate all locations
            int count = 0;
            foreach (Boid other in boids)
            {
                float d = calcDistance(location, other.location);
                if ((d > 0) && (d < neighbordist))
                {
                    sum += (other.location); // Add location
                    count++;
                }
            }
            if (count > 0)
            {
                sum = calcDiv(sum, count);
                return seek(sum);  // Steer towards the location
            }
            else
            {
                return new Vector2(0, 0);
            }

        }

        private Vector2 seek(Vector2 target)
        {

            Vector2 desired = (target - location);  // A vector pointing from the location to the target
            // Normalize desired and scale to maximum speed
            desired.Normalize();
            desired *= (maxSpeed);
            // Steering = Desired minus Velocity
            Vector2 steer = (desired - velocity);
            steer = limitVector(steer, maxForce);  // Limit to maximum steering force
            return steer;
        }

        private float calcMag(Vector2 vec)
        {
            return (float)Math.Sqrt((vec.X * vec.X) + (vec.Y * vec.Y));

        }


        private Vector2 limitVector(Vector2 vec, float limit)
        {
            vec.Normalize();
            vec *= limit;
            return vec;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Begin();
            spritebatch.Draw(image, location, null, Color.White, rotation, new Vector2(image.Width / 2, image.Height / 2), 1f, SpriteEffects.None, 0f);
            spritebatch.End();
        }

    }
}
