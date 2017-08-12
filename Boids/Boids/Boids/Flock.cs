using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids
{
    public class Flock
    {
        List<Boid> Boids;
        public static Random r=new Random();
       
        
        public Flock() { 
        
        Boids = new List<Boid>();
        }


        public void Update(GameTime gametime){
        
            foreach(Boid b in Boids){

                b.Update(gametime,Boids);
        
        }
        
        }

        public void Draw(SpriteBatch spritebatch)
        {
            foreach (Boid b in Boids)
            {
                b.Draw(spritebatch);

            }
        }
        public void AddBoid(Boid b) {
            Boids.Add(b);
        
        }


    }
}
