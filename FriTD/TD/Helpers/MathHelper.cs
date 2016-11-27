using System;

namespace TD.Helpers
{
    class MathHelper
    {
        public static int DistanceBetweenPoints(int x1, int y1, int x2, int y2)
        {
            var diffX = x2 - x1;
            var diffY = y2 - y1;

            return (int)Math.Sqrt(diffX * diffX + diffY * diffY);
        }

        public static double DistanceBetweenPoints(double x1, double y1, double x2, double y2)
        {
            var diffX = x2 - x1;
            var diffY = y2 - y1;

            return Math.Sqrt(diffX * diffX + diffY * diffY);
        }
    }
}
