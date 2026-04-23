using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace breakout
{
    public class Block
    {
        public double X { get; }
        public double Y { get; }
        public double Width { get; }
        public double Height { get; }

        public int Health { get; private set; }

        public bool IsDestroyed => Health <= 0;

        public Block(double x, double y, int health)
        {
            X = x;
            Y = y;

            Width = 60;
            Height = 20; 

            Health = health;
        }

        public void Hit() => Health--;

        
    }
}
