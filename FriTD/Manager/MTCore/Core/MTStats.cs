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
        public static int[] Level = new int[20];
        public static int[] TypeW = new int[2];
        public static int[] TypeL = new int[2];

        // TODO: refactor
        public static int[] WPerMap = new int[10];
        public static int[] LPerMap = new int[10];
        public static int[] WPerMapTotal = new int[10];
        public static int[] LPerMapTotal = new int[10];

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void IncWl(int wl, int level, int mapNumber = 0)
        {
            Level[level]++;
            Total++;

            if (wl == 0)
            {
                Lost++;
                TotalLost++;
                LPerMap[mapNumber]++;
                LPerMapTotal[mapNumber]++;
            }
            else
            {
                Won++;
                TotalWon++;
                WPerMap[mapNumber]++;
                WPerMapTotal[mapNumber]++;
            }

            if ((Lost + Won) % 1000 == 0)
            {
                Console.WriteLine(@"Iteration: {0} Won: {1} Lost: {2}", Total, Won, Lost);
                //Console.WriteLine(@"Iteration: {0} Won: {1} Lost: {2} -W {3}  {4} -L {5}  {6}", Total, Won, Lost, TypeW[0], TypeW[1], TypeL[0], TypeL[1]);
                //Console.WriteLine(Won);

                var perMap = "\tW/L per map type: ";
                for (int i = 0; i < WPerMap.Length; i++)
                {
                    if (!(WPerMap[i] == 0 && LPerMap[i] == 0))
                    {
                        perMap += i + ":(" + WPerMap[i] + ":" + LPerMap[i] + "); ";
                    }
                }
                Console.WriteLine(perMap);

                perMap = "\tW/L per map type (total): ";
                for (int i = 0; i < WPerMapTotal.Length; i++)
                {
                    if (!(WPerMapTotal[i] == 0 && LPerMapTotal[i] == 0))
                    {
                        perMap += i + ":(" + WPerMapTotal[i] + ":" + LPerMapTotal[i] + "); ";
                    }
                }
                Console.WriteLine(perMap);

                Reset();
            }
        }

        public static void PrintLevelsOfEnding()
        {
            Console.WriteLine(@"levels");
            foreach (int lvlItem in Level)
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
            WPerMap = new int[10];
            LPerMap = new int[10];
        }

        public static void ResetAll()
        {
            Console.WriteLine(@"########################## RESETTING STATISTICS ##########################");
            Reset();
            Level = new int[20];
            WPerMapTotal = new int[10];
            LPerMapTotal = new int[10];
        }

        internal static void IncWl(int v, int level, int type, int mapNumber = 0)
        {
            if (v == 0)
            {
                TypeL[type]++;
            }
            else
            {
                TypeW[type]++;
            }
            IncWl(v, level, mapNumber);
        }

        public static void PrintTotalScore()
        {
            Console.WriteLine(@"Total score: w {0} l {1}", TotalWon, TotalLost);
        }
    }
}
