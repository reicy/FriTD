using System;
using System.Dynamic;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Manager.MTCore
{
    public class MtStats
    {

        public static int Won { get; set; }
        public static int Lost { get; set; }
        public static int Total { get; set; }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void IncWL(int wl)
        {
            Total++;
            if (wl == 0)
            {
                Lost++;
                
            }
            else
            {
                Won++;
            }
            if ((Lost + Won)%100 == 0)
            {
                Console.WriteLine("Iteration: {0} Won: {1} Lost: {2}", Total, Won, Lost);
                Reset();
            }
        }
        

        public static void Reset()
        {
            Won = 0;
            Lost = 0;
        }
    }
}