using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace breakout
{
    public class Ball
    {
        private double x;
        private double y;

        private double dx;
        private double dy;

        public double Size { get; }

        public Ball(double startX, double startY)
        {
            x = startX;
            y = startY;

            dx = 3;
            dy = 3;

            Size = 10;
        }
        public double X => x;
        public double Y => y;

        public void Move()
        {
            x += dx;
            y += dy;
        }

        public void BounceX()
        {
            dx = -dx;
        }

        public void BounceY()
        {
            dy = -dy;
        }

        public void SetPosition(double newX, double newY)
        {
            x = newX;
            y = newY;
        }

        public void SetDirection(double newDx, double newDy)
        {
            dx = newDx;
            dy = newDy;
        }

        public void IncreaseSpeed(double newSpeed)
        {
            double length = Math.Sqrt(dx * dx + dy * dy);

            dx = (dx / length) * newSpeed;
            dy = (dy / length) * newSpeed;
        }

        public double GetSpeed()
        {
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
