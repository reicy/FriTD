using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using static Manager.Utils.CustomLogger;

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
        private static int[] WPerMap = new int[10];
        private static int[] LPerMap = new int[10];
        private static int[] WPerMapTotal = new int[10];
        private static int[] LPerMapTotal = new int[10];
        private static int[,] LevelPerMapTotal = new int[10, 20];
        private static int[,] LevelPerMap = new int[10, 20];

        private static readonly object _lock = new Object();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void IncWl(int wl, int level, int mapNumber = 0)
        {
            lock (_lock)
            {
                Level[level]++;
                Total++;

                if (wl == 0)
                {
                    Lost++;
                    TotalLost++;
                    LPerMap[mapNumber]++;
                    LPerMapTotal[mapNumber]++;
                    LevelPerMap[mapNumber, level]++;
                    LevelPerMapTotal[mapNumber, level]++;
                }
                else
                {
                    Won++;
                    TotalWon++;
                    WPerMap[mapNumber]++;
                    WPerMapTotal[mapNumber]++;
                }

                if ((Lost + Won)%1000 == 0)
                {
                    Log(@"Iteration: " + Total + " Won: " + Won + " Lost: " + Lost);
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
                    Log(perMap);

                    perMap = "\tW/L per map type (total): ";
                    for (int i = 0; i < WPerMapTotal.Length; i++)
                    {
                        if (!(WPerMapTotal[i] == 0 && LPerMapTotal[i] == 0))
                        {
                            perMap += i + ":(" + WPerMapTotal[i] + ":" + LPerMapTotal[i] + "); ";
                        }
                    }
                    Log(perMap);
                    Reset();
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void PrintLevelsOfEnding()
        {
            /*
            Console.WriteLine(@"levels");
            foreach (int lvlItem in Level)
            {
                Console.Write(@"{0} ", lvlItem);
            }
            */
            Log("Lost in levels: ");
            for (int i = 0; i < LevelPerMapTotal.GetLength(0); i++)
            {
                Log(i + ":[", false);
                for (int k = 0; k < LevelPerMapTotal.GetLength(1); k++)
                {
                    Log(LevelPerMapTotal[i, k] + ", ", false);
                }
                Log("]; ");
            }
            Log();
            Log();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Reset()
        {
            Won = 0;
            Lost = 0;
            TypeW = new[] {0, 0};
            TypeL = new[] {0, 0};
            WPerMap = new int[10];
            LPerMap = new int[10];
            LevelPerMap = new int[10, 20];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void ResetAll()
        {
            PrintLevelsOfEnding();
            Log(@"########################## RESETTING STATISTICS ##########################");
            Reset();
            Level = new int[20];
            WPerMapTotal = new int[10];
            LPerMapTotal = new int[10];
            LevelPerMapTotal = new int[10, 20];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void PrintTotalScore()
        {
            Log(@"Total score: w " + TotalWon + " l " + TotalLost);
        }
    }
}
