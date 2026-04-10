using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace breakout
{
    public class Paddle
    {
        private double x;

        public double Y { get; }
        public double Width {  get; }
        public double Heigth { get; }

        public double X => x;

        public Paddle(double startX, double startY)
        {
            x = startX;
            Y = startY;

            Width = 100;
            Heigth = 15;
        }


        public void MoveTo(double mouseX, double canvasWidth)
        {
            x = mouseX - Width / 2;

            if (x < 0) x = 0;
            if (x + Width > canvasWidth) x = canvasWidth - Width;
        }
    }
}
