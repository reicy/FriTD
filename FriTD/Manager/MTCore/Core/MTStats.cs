using static Manager.Utils.CustomLogger;

namespace Manager.MTCore.Core
{
    public static class MtStats
    {
        public const int MAP_COUNT = 6;

        public static int Won { get; private set; }
        public static int Lost { get; private set; }
        public static int Total { get; private set; }
        public static int TotalWon { get; private set; }
        public static int TotalLost { get; private set; }

        private static int _highestMapNumber = 0;
        private static int[] _level = new int[20];
        private static int[] _typeW = new int[2];
        private static int[] _typeL = new int[2];
        private static int[] _wPerMap = new int[10];
        private static int[] _lPerMap = new int[10];
        private static int[] _wPerMapTotal = new int[10];
        private static int[] _lPerMapTotal = new int[10];
        private static int[,] _levelPerMapTotal = new int[10, 20];
        private static int[,] _levelPerMap = new int[10, 20];

        private static readonly object Lock1 = new object();
        private static readonly object Lock2 = new object();
        private static readonly object Lock3 = new object();
        private static readonly object Lock4 = new object();
        private static readonly object Lock5 = new object();
        private static readonly object Lock6 = new object();

        public static void IncWl(int wl, int level, int type = -1, int mapNumber = 0)
        {
            lock (Lock1)
            {
                /*if (type >= 0)
                {
                    if (wl == 0)
                    {
                        _typeL[type]++;
                    }
                    else
                    {
                        _typeW[type]++;
                    }
                }*/

                if (_highestMapNumber < mapNumber) _highestMapNumber = mapNumber;
                _level[level]++;
                Total++;

                if (wl == 0)
                {
                    Lost++;
                    TotalLost++;
                    _lPerMap[mapNumber]++;
                    _lPerMapTotal[mapNumber]++;
                    _levelPerMap[mapNumber, level]++;
                    _levelPerMapTotal[mapNumber, level]++;
                }
                else
                {
                    Won++;
                    TotalWon++;
                    _wPerMap[mapNumber]++;
                    _wPerMapTotal[mapNumber]++;
                }

                if ((Lost + Won) % 1000 == 0)
                {
                    Log($@"Iteration: {Total} Won: {Won} Lost: {Lost}");
                    //Console.WriteLine(@"Iteration: {0} Won: {1} Lost: {2} -W {3}  {4} -L {5}  {6}", Total, Won, Lost, _typeW[0], _typeW[1], _typeL[0], _typeL[1]);
                    //Console.WriteLine(Won);

                    var perMap = "\tW/L per map type: ";
                    for (int i = 0; i <= _highestMapNumber; i++)
                    {
                        if (!(_wPerMap[i] == 0 && _lPerMap[i] == 0))
                        {
                            perMap += $"{i}:({_wPerMap[i]}:{_lPerMap[i]}); ";
                        }
                    }
                    Log(perMap);

                    perMap = "\tW/L per map type (total): ";
                    for (int i = 0; i <= _highestMapNumber; i++)
                    {
                        if (!(_wPerMapTotal[i] == 0 && _lPerMapTotal[i] == 0))
                        {
                            perMap += $"{i}:({_wPerMapTotal[i]}:{_lPerMapTotal[i]}); ";
                        }
                    }
                    Log(perMap);
                    Reset();
                }
            }
        }

        public static void PrintLevelsOfEnding()
        {
            lock (Lock2)
            {
                /*
                Console.WriteLine(@"levels");
                foreach (int lvlItem in _level)
                {
                    Console.Write(@"{0} ", lvlItem);
                }
                */
                Log("Lost in levels: ");
                for (int i = 0; i <= _highestMapNumber; i++)
                {
                    Log($"{i}:[", false);
                    for (int k = 0; k < _levelPerMapTotal.GetLength(1); k++)
                    {
                        Log($"{_levelPerMapTotal[i, k]}, ", false);
                    }
                    Log("]; ");
                }
                Log();
                Log();
            }
        }

        public static void Reset()
        {
            lock (Lock3)
            {
                Won = 0;
                Lost = 0;
                _typeW = new[] { 0, 0 };
                _typeL = new[] { 0, 0 };
                _wPerMap = new int[10];
                _lPerMap = new int[10];
                _levelPerMap = new int[10, 20];
            }
        }

        public static void ResetAll()
        {
            PrintLevelsOfEnding();
            Log(@"########################## RESETTING STATISTICS ##########################");
            Reset();
            lock (Lock4)
            {
                _level = new int[20];
                _wPerMapTotal = new int[10];
                _lPerMapTotal = new int[10];
                _levelPerMapTotal = new int[10, 20];
                Total = 0;
                TotalWon = 0;
                TotalLost = 0;
            }
        }

        public static string ToCsvString()
        {
            string ret;

            lock (Lock5)
            {
                ret = $"{TotalWon}, {TotalLost}, {((TotalWon * 1.0 / (TotalWon + TotalLost)) * 100).ToString("F")}%, ";

                for (int i = 0; i < MAP_COUNT; i++)
                {
                    int total = _wPerMapTotal[i] + _lPerMapTotal[i];
                    if (total == 0) total = 1;
                    ret +=
                        $"{_wPerMapTotal[i]}, {_lPerMapTotal[i]}, {((_wPerMapTotal[i] * 1.0 / (total)) * 100).ToString("F")}%, ";
                }
            }

            return ret;
        }

        public static void PrintTotalScore()
        {
            lock (Lock6)
            {
                Log($@"Total score: w {TotalWon} l {TotalLost}");
            }
        }
    }
}
