using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
