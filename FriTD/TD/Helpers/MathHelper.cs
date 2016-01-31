using System;

namespace TD.Helpers
{
    class MathHelper
    {
        public static int DistanceBetweenPoints(int x1, int y1, int x2, int y2)
        {

            int diffX, diffY;
            diffX = x2 - x1;
            diffY = y2 - y1;

            return (int)Math.Sqrt(diffX * diffX + diffY * diffY);

        }
    }
}
