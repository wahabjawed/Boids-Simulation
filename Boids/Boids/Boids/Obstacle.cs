using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Boids
{
    public class Obstacle
    {
        Texture2D image;
        Vector2 position;
        public Obstacle(Texture2D _image,Vector2 _position)
        {
            image = _image;
            position = _position;
        }
    }
}
