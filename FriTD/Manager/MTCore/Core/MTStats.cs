using System;
using System.Runtime.CompilerServices;

namespace Manager.MTCore.Core
{
    public class MtStats
    {
        public static int Won { get; set; }
        public static int Lost { get; set; }
        public static int Total { get; set; }
        public static int TotalWon { get; set; }
        public static int TotalLost { get; set; }
        public static int[] Level = new int[10];
        public static int[] TypeW = new int[2];
        public static int[] TypeL = new int[2];

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void IncWl(int wl, int level)
        {
            Level[level]++;
            Total++;
            if (wl == 0)
            {
                Lost++;
                TotalLost++;
            }
            else
            {
                Won++;
                TotalWon++;
            }
            if ((Lost + Won) % 100 == 0)
            {
                //Console.WriteLine(@"Iteration: {0} Won: {1} Lost: {2}", Total, Won, Lost);
                //Console.WriteLine(@"Iteration: {0} Won: {1} Lost: {2} -W {3}  {4} -L {5}  {6}", Total, Won, Lost, TypeW[0], TypeW[1], TypeL[0], TypeL[1]);
                //Console.WriteLine(Won);
                Reset();
            }
        }

        public static void PrintLevelsOfEnding()
        {
            Console.WriteLine(@"levels");
            foreach (var lvlItem in Level)
            {
                Console.Write(@"{0} ", lvlItem);
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        public static void Reset()
        {
            Won = 0;
            Lost = 0;
            TypeW = new[] { 0, 0 };
            TypeL = new[] { 0, 0 };
        }

        internal static void IncWl(int v, int level, int type)
        {
            if (v == 0)
            {
                TypeL[type]++;
            }
            else
            {
                TypeW[type]++;
            }
            IncWl(v, level);
        }

        public static void PrintTotalScore()
        {
            Console.WriteLine(@"Total score: w {0} l {1}", TotalWon, TotalLost);
        }
    }
}