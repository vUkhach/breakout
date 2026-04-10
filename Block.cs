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

        public bool IsDestroyed { get; private set; }

        public Block(double x, double y)
        {
            X = x;
            Y = y;

            Width = 60;
            Height = 20; 

            IsDestroyed = false;
        }

        public void Destroy()
        {
            IsDestroyed = true;
        }

        
    }
}
