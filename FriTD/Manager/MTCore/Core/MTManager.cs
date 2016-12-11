using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Manager.Kohonen;
using Manager.QLearning;
using static Manager.Utils.CustomLogger;

namespace Manager.MTCore.Core
{
    public class MtManager
    {
        private bool _loadKohonen = false;
        private string _kohonenLoadPath = @"C:\Users\Tomas\Desktop\kohonenLearnt\M010KE.txt";
        // @"C:\Users\Tomas\Desktop\kohonenLearnt\cosFixed5k.txt";
        // @"C:\Users\Tomas\Desktop\kohonenLearnt\k24kForM1.txt";

        // @"C:\Users\Tomas\Desktop\kohonenLearnt\k20kCosM0.txt";// @"C:\Users\Tomas\Desktop\kohonenLearnt\Map0K20k.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\k20kHeuriM0.txt"; // @"C:\Users\Tomas\Desktop\kohonenLearnt\CTest.txt";////@"C:\Users\Tomas\Desktop\kohonenLearnt\k24kForM1.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\MixedKohonen24_4_2.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\KMixed24_M1_4_M0.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\k24kForM1.txt";// @"C:\Users\Tomas\Desktop\kohonenLearnt\K3kForMMEfekt2.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\Kohonen3kForM0MEfekt.txt";
        private bool _nonEmptyModeCohonenActive = false;
        private int _numOfType2 = 0;

        private const int THREADS_NUM = 6;
        private const int ITERATIONS = 10000;
        private const int ITERATION_STOP_KOHONEN_UPDATE = 30000;
        private const int ITERATION_OF_SINGLE_THREAD_START_LEARNING = 5;
        private const int QUEUE_MAX_CAPACTITY = 1;

        private static readonly bool CosDistActive = true;
        private static readonly bool HeuristicActive = true;

        public void ProcessLearning()
        {
            for (var i = 0; i < 10; i++)
                SingleRun();

            //init core
            var qLearning = new QLearning<KohonenAiState, AI.Action>(0.3, 1, 0.5);
            var kohonen = new KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5, _nonEmptyModeCohonenActive);

            //load kohonen
            if (_loadKohonen)
                kohonen.Load(_kohonenLoadPath);

            //var weight = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 100, 0.001, 1000, 1, 1, 1, 1, 1, 1, 1, 1};
            //var weight = new double[] { 1000000, 1000000, 1000000, 10000, 10000, 10000, 100, 100, 1, 1, 0.001, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            //var weight = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1000000, 100, 0, 1, 1, 1, 1, 1, 1, 1, 1 };
            var weight = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            kohonen.SetWeight(weight);
            var threads = new List<Thread>();
            var daemons = new List<MtSingleDaemon>();
            var kohonenUpdateQueues = new List<BlockingCollection<KohonenUpdate>>();
            var kohonenUpdatesToProcess = new List<KohonenUpdate>();

            var won = 0;
            var lost = 0;
            //var counter = 0;

            Console.WriteLine(DateTime.Now);

            //init threads
            for (var i = 0; i < THREADS_NUM; i++)
            {
                var queue = new BlockingCollection<KohonenUpdate>(QUEUE_MAX_CAPACTITY);
                MtSingleDaemon singleDaemon;

                if (i < _numOfType2)
                {
                    Console.WriteLine(@"map1");
                    singleDaemon = new MtSingleDaemon(kohonen, qLearning, queue, Properties.Resources.Map1, Properties.Resources.Levels1, 1, HeuristicActive, CosDistActive)
                    {
                        IterationStartLearning = ITERATION_OF_SINGLE_THREAD_START_LEARNING
                    };
                }
                else
                {
                    Console.WriteLine(@"map");
                    singleDaemon = new MtSingleDaemon(kohonen, qLearning, queue, Properties.Resources.Map, Properties.Resources.Levels, 0, HeuristicActive, CosDistActive)
                    {
                        IterationStartLearning = ITERATION_OF_SINGLE_THREAD_START_LEARNING
                    };
                }

                var thread = new Thread(singleDaemon.ProcessLearning);
                threads.Add(thread);
                daemons.Add(singleDaemon);
                kohonenUpdateQueues.Add(queue);
            }

            //start threads
            foreach (var thread in threads)
            {
                thread.Start();
                while (!thread.IsAlive) { }
            }

            //init stopwatch
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            //Core cycle
            //execute level -> update kohonen -> repeat
            for (var i = 0; i < ITERATIONS; i++)
            {
                won = 0;
                lost = 0;

                //collect kohonen updates
                foreach (var queue in kohonenUpdateQueues)
                {
                    var up = queue.Take();
                    kohonenUpdatesToProcess.Add(up);
                }

                //wait for threads to stop
                foreach (var queue in kohonenUpdateQueues)
                    while (queue.Count != QUEUE_MAX_CAPACTITY) { }

                //update kohonen
                foreach (var update in kohonenUpdatesToProcess)
                    if (ITERATION_STOP_KOHONEN_UPDATE > i) kohonen.ReArrange(update.Row, update.Col, update.Vector);
                if (ITERATION_STOP_KOHONEN_UPDATE == i)
                    kohonen.ResetAccesses();
                kohonenUpdatesToProcess.Clear();

                foreach (var mtSingleDaemon in daemons)
                {
                    won += mtSingleDaemon.Won;
                    lost += mtSingleDaemon.Lost;
                }
            }

            // stop watch and displ time elapsed
            stopWatch.Stop();

            var ts = stopWatch.Elapsed;
            Console.WriteLine(@"RunTime {0:hh\:mm\:ss\.ff}", ts);
            MtStats.PrintLevelsOfEnding();
            MtStats.PrintTotalScore();

            kohonen.PrintAccesses();
            kohonen.Displ();
            kohonen.Displ(11);

            //Console.WriteLine(@"won: {0} lost: {1}", won, lost);

            Console.WriteLine(DateTime.Now);
            qLearning.QValDisp();

            //Dispose threads in inhuman manner, TODO change to more human approach
            foreach (var thread in threads)
                thread.Abort();
        }

        public void SingleRun()
        {
            //init core
            var qLearning = new QLearning<KohonenAiState, AI.Action>(0.3, 1, 0.5);
            var kohonen = new KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5, _nonEmptyModeCohonenActive);

            //load kohonen
            if (_loadKohonen)
                kohonen.Load(_kohonenLoadPath);

            //var weight = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 100, 0.001, 1000, 1, 1, 1, 1, 1, 1, 1, 1};
            //var weight = new double[] { 1000000, 1000000, 1000000, 10000, 10000, 10000, 100, 100, 1, 1, 0.001, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            //var weight = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1000000, 100, 0, 1, 1, 1, 1, 1, 1, 1, 1 };
            var weight = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            kohonen.SetWeight(weight);
            var threads = new List<Thread>();
            var daemons = new List<MtSingleDaemon>();
            var kohonenUpdateQueues = new List<BlockingCollection<KohonenUpdate>>();
            var kohonenUpdatesToProcess = new List<KohonenUpdate>();

            var won = 0;
            var lost = 0;
            //var counter = 0;

            Console.WriteLine(DateTime.Now);

            //init threads
            for (var i = 0; i < THREADS_NUM; i++)
            {
                var queue = new BlockingCollection<KohonenUpdate>(QUEUE_MAX_CAPACTITY);
                MtSingleDaemon singleDaemon;

                if (i < _numOfType2)
                {
                    Console.WriteLine(@"map1");
                    singleDaemon = new MtSingleDaemon(kohonen, qLearning, queue, Properties.Resources.Map1, Properties.Resources.Levels1, 1, HeuristicActive, CosDistActive)
                    {
                        IterationStartLearning = ITERATION_OF_SINGLE_THREAD_START_LEARNING
                    };
                }
                else
                {
                    Console.WriteLine(@"map");
                    singleDaemon = new MtSingleDaemon(kohonen, qLearning, queue, Properties.Resources.Map, Properties.Resources.Levels, 0, HeuristicActive, CosDistActive)
                    {
                        IterationStartLearning = ITERATION_OF_SINGLE_THREAD_START_LEARNING
                    };
                }

                var thread = new Thread(singleDaemon.ProcessLearning);
                threads.Add(thread);
                daemons.Add(singleDaemon);
                kohonenUpdateQueues.Add(queue);
            }

            //start threads
            foreach (var thread in threads)
            {
                thread.Start();
                while (!thread.IsAlive) { }
            }

            //init stopwatch
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            //Core cycle
            //execute level -> update kohonen -> repeat
            for (var i = 0; i < ITERATIONS; i++)
            {
                won = 0;
                lost = 0;

                //collect kohonen updates
                foreach (var queue in kohonenUpdateQueues)
                {
                    var up = queue.Take();
                    kohonenUpdatesToProcess.Add(up);
                }

                //wait for threads to stop
                foreach (var queue in kohonenUpdateQueues)
                    while (queue.Count != QUEUE_MAX_CAPACTITY) { }

                //update kohonen
                foreach (var update in kohonenUpdatesToProcess)
                    if (ITERATION_STOP_KOHONEN_UPDATE > i) kohonen.ReArrange(update.Row, update.Col, update.Vector);
                if (ITERATION_STOP_KOHONEN_UPDATE == i)
                    kohonen.ResetAccesses();
                kohonenUpdatesToProcess.Clear();

                foreach (var mtSingleDaemon in daemons)
                {
                    won += mtSingleDaemon.Won;
                    lost += mtSingleDaemon.Lost;
                }
            }

            // stop watch and displ time elapsed
            stopWatch.Stop();

            var ts = stopWatch.Elapsed;
            Console.WriteLine(@"RunTime {0:hh\:mm\:ss\.ff}", ts);
            MtStats.PrintLevelsOfEnding();
            MtStats.PrintTotalScore();

            //Dispose threads in inhuman manner
            foreach (var thread in threads)
                thread.Abort();
        }

        public void ExperimentRun(List<string> maps, int numberOfIterationsPerMap,
            int numberOfIterationsPerMapWithKohonen, bool useCosDist = false,
            double qLearningRandomActionProbability = 0.3,
            double qLearningDiscountFactor = 1, double qLearningLearningRate = 0.5, int kohonenRows = 30,
            int kohonenCols = 30, double kohonenRadius = 2, double kohonenLearningRate = 0.5,
            double kohonenDistFactor = 1, string kohonenLoadFile = null, string qLearningLoadFile = null,
            string kohonenSaveFile = null, string qLearningSaveFile = null)
        {
            var qLearning = new QLearning<KohonenAiState, AI.Action>(qLearningRandomActionProbability,
                qLearningDiscountFactor, qLearningLearningRate);
            var kohonen = new KohonenCore<StateVector>(kohonenRows, kohonenCols, kohonenRadius, kohonenLearningRate,
                kohonenDistFactor, 1, 0.5, _nonEmptyModeCohonenActive);
            var weight = new double[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
            kohonen.SetWeight(weight);
            var threads = new List<Thread>();
            var daemons = new List<MtSingleDaemon>();
            var kohonenUpdateQueues = new List<BlockingCollection<KohonenUpdate>>();
            var kohonenUpdatesToProcess = new List<KohonenUpdate>();

            if (kohonenLoadFile != null)
            {
                kohonen.Load(kohonenLoadFile);
            }

            if (qLearningLoadFile != null)
            {
                qLearning.Load(qLearningLoadFile);
            }

            string mapsString = "";
            foreach (var map in maps)
            {
                mapsString += map + "  ";
                var mapAndLevel = getMapAndLevel(map);
                CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, mapAndLevel.Key, mapAndLevel.Value,
                    getnumberOfMap(map), threads, daemons, kohonenUpdateQueues, CosDistActive);
            }

            Log(@"############ STARTING #############");
            Log("Maps: " + mapsString);
            Log("Number of iterations per map: " + numberOfIterationsPerMap + "; with kohonen: " +
                numberOfIterationsPerMapWithKohonen);
            Log("Total number of iterations per map: " + numberOfIterationsPerMap*maps.Count + "; with kohonen: " +
                numberOfIterationsPerMapWithKohonen*maps.Count);
            Log("Cos distance: " + useCosDist);
            Log();
            Log("KOHONEN");
            Log("rows: " + kohonenRows + "; cols: " + kohonenCols + "; radius: " + kohonenRadius);
            Log("learning rate: " + kohonenLearningRate + "; disFactor: " + kohonenDistFactor);
            Log();
            Log("QLEARNING");
            Log("random action prob: " + qLearningRandomActionProbability + "; disFactor: " + qLearningDiscountFactor +
                "; learning rate: " + qLearningLearningRate);
            Log("####################################");

            StartMagic(kohonen, numberOfIterationsPerMap, numberOfIterationsPerMapWithKohonen, threads, daemons,
                kohonenUpdateQueues, kohonenUpdatesToProcess);

            MtStats.PrintLevelsOfEnding();
            MtStats.PrintTotalScore();
            MtStats.ResetAll();

            if (kohonenSaveFile != null)
            {
                kohonen.Save(kohonenSaveFile);
            }

            if (qLearningSaveFile != null)
            {
                qLearning.Save(qLearningSaveFile);
            }
        }

        // run 6 types of maps from one kohonen and qlearning
        public void ExperimentRun1()
        {
            // TODO: interationstart learning in MTSingleDaemon

            // create qlearning and kohonen and init other very important things...
            var qLearning = new QLearning<KohonenAiState, AI.Action>(0.3, 1, 0.5);
            var kohonen = new KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5, _nonEmptyModeCohonenActive);
            var weight = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            kohonen.SetWeight(weight);
            var threads = new List<Thread>();
            var daemons = new List<MtSingleDaemon>();
            var kohonenUpdateQueues = new List<BlockingCollection<KohonenUpdate>>();
            var kohonenUpdatesToProcess = new List<KohonenUpdate>();

            // init threads
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map, Properties.Resources.Levels, 0, threads, daemons, kohonenUpdateQueues);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map1, Properties.Resources.Levels1, 1, threads, daemons, kohonenUpdateQueues);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map2, Properties.Resources.Levels2, 2, threads, daemons, kohonenUpdateQueues);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map3, Properties.Resources.Levels3, 3, threads, daemons, kohonenUpdateQueues);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map4, Properties.Resources.Levels4, 4, threads, daemons, kohonenUpdateQueues);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map5, Properties.Resources.Levels5, 5, threads, daemons, kohonenUpdateQueues);

            Console.WriteLine(@"\n\n ########################## \nSTARTING 6 MAPS in 6 THREADS WITH KOHONEN LEARNING TURNED ON\nSettings: QLearning<KohonenAiState>(0.3, 1, 0.5); KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5, nonEmptyModeCohonenActive); \n ########################## ");
            StartMagic(kohonen, 2500, 2500, threads, daemons, kohonenUpdateQueues, kohonenUpdatesToProcess);

            MtStats.PrintLevelsOfEnding();
            MtStats.PrintTotalScore();
            MtStats.ResetAll();

            qLearning.Epsilon = 0.1;
            kohonen.LearningRate = 0.3;
            threads.Clear();
            daemons.Clear();
            kohonenUpdateQueues.Clear();

            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map, Properties.Resources.Levels, 0, threads, daemons, kohonenUpdateQueues);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map1, Properties.Resources.Levels1, 1, threads, daemons, kohonenUpdateQueues);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map2, Properties.Resources.Levels2, 2, threads, daemons, kohonenUpdateQueues);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map3, Properties.Resources.Levels3, 3, threads, daemons, kohonenUpdateQueues);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map4, Properties.Resources.Levels4, 4, threads, daemons, kohonenUpdateQueues);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map5, Properties.Resources.Levels5, 5, threads, daemons, kohonenUpdateQueues);

            Console.WriteLine(@"\n\n ########################## \nSTARTING 6 MAPS in 6 THREADS WITH KOHONEN LEARNING TURNED OFF\nSettings: QLearning<KohonenAiState>(0.1, 1, 0.5); KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5, nonEmptyModeCohonenActive); \n ########################## ");
            StartMagic(kohonen, 2500, 1250, threads, daemons, kohonenUpdateQueues, kohonenUpdatesToProcess);

            MtStats.PrintLevelsOfEnding();
            MtStats.PrintTotalScore();
            MtStats.ResetAll();

            qLearning.Alpha = 0.2;
            threads.Clear();
            daemons.Clear();
            kohonenUpdateQueues.Clear();

            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map, Properties.Resources.Levels, 0, threads, daemons, kohonenUpdateQueues);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map1, Properties.Resources.Levels1, 1, threads, daemons, kohonenUpdateQueues);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map2, Properties.Resources.Levels2, 2, threads, daemons, kohonenUpdateQueues);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map3, Properties.Resources.Levels3, 3, threads, daemons, kohonenUpdateQueues);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map4, Properties.Resources.Levels4, 4, threads, daemons, kohonenUpdateQueues);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map5, Properties.Resources.Levels5, 5, threads, daemons, kohonenUpdateQueues);

            Console.WriteLine(@"\n\n ########################## \nSTARTING 6 MAPS in 6 THREADS WITH KOHONEN LEARNING TURNED OFF\nSettings: QLearning<KohonenAiState>(0.1, 1, 0.2); KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5, nonEmptyModeCohonenActive); \n ########################## ");
            StartMagic(kohonen, 2500, 0, threads, daemons, kohonenUpdateQueues, kohonenUpdatesToProcess);

            MtStats.PrintLevelsOfEnding();
            MtStats.PrintTotalScore();
        }

        // run 6 types of maps from one kohonen and qlearning
        public void ExperimentRun2()
        {
            // TODO: interationstart learning in MTSingleDaemon

            // create qlearning and kohonen and init other very important things...
            var qLearning = new QLearning<KohonenAiState, AI.Action>(0.3, 1, 0.5);
            var kohonen = new KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5, _nonEmptyModeCohonenActive);
            var weight = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            kohonen.SetWeight(weight);
            var threads = new List<Thread>();
            var daemons = new List<MtSingleDaemon>();
            var kohonenUpdateQueues = new List<BlockingCollection<KohonenUpdate>>();
            var kohonenUpdatesToProcess = new List<KohonenUpdate>();
            var costDist = true;

            kohonen.Load("kohonenMap1.dat");

            // init threads
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map, Properties.Resources.Levels, 0, threads, daemons, kohonenUpdateQueues, costDist);
            /*CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map1, Properties.Resources.Levels1, 1, threads, daemons, kohonenUpdateQueues, costDist);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map2, Properties.Resources.Levels2, 2, threads, daemons, kohonenUpdateQueues, costDist);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map3, Properties.Resources.Levels3, 3, threads, daemons, kohonenUpdateQueues, costDist);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map4, Properties.Resources.Levels4, 4, threads, daemons, kohonenUpdateQueues, costDist);
            CreateMtDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map5, Properties.Resources.Levels5, 5, threads, daemons, kohonenUpdateQueues, costDist);*/

            Console.WriteLine(@"\n\n ########################## \nSTARTING 1 MAPS in 1 THREADS WITH KOHONEN LEARNING TURNED ON\nSettings: QLearning<KohonenAiState>(0.3, 1, 0.5); KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5, nonEmptyModeCohonenActive); \n ########################## ");
            StartMagic(kohonen, 5000, 5001, threads, daemons, kohonenUpdateQueues, kohonenUpdatesToProcess);

            MtStats.PrintLevelsOfEnding();
            MtStats.PrintTotalScore();

            kohonen.Save("k2");
            qLearning.Save("q2");
        }

        public void StartMagic(KohonenCore<StateVector> kohonen, int numOfIterationsPerThread, int numOfIterationsWithKohonenLearningPerThread, List<Thread> threads, List<MtSingleDaemon> daemons, List<BlockingCollection<KohonenUpdate>> kohonenUpdateQueues, List<KohonenUpdate> kohonenUpdatesToProcess)
        {
            var won = 0;
            var lost = 0;
            //var counter = 0;

            //start threads
            foreach (var thread in threads)
            {
                thread.Start();
                while (!thread.IsAlive) { }
            }

            //init stopwatch
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            //Core cycle
            //execute level -> update kohonen -> repeat
            for (var i = 0; i < numOfIterationsPerThread; i++)
            {
                won = 0;
                lost = 0;

                //collect kohonen updates
                foreach (var queue in kohonenUpdateQueues)
                {
                    var up = queue.Take();
                    kohonenUpdatesToProcess.Add(up);
                }

                //wait for threads to stop
                foreach (var queue in kohonenUpdateQueues)
                    while (queue.Count != QUEUE_MAX_CAPACTITY) { }

                //update kohonen
                foreach (var update in kohonenUpdatesToProcess)
                    if (numOfIterationsWithKohonenLearningPerThread > i) kohonen.ReArrange(update.Row, update.Col, update.Vector);
                if (numOfIterationsWithKohonenLearningPerThread == i)
                    kohonen.ResetAccesses();
                kohonenUpdatesToProcess.Clear();

                foreach (var mtSingleDaemon in daemons)
                {
                    won += mtSingleDaemon.Won;
                    lost += mtSingleDaemon.Lost;
                }
            }

            // stop watch and displ time elapsed
            stopWatch.Stop();

            var ts = stopWatch.Elapsed;
            Console.WriteLine(@"RunTime {0:hh\:mm\:ss\.ff}", ts);

            //Dispose threads in inhuman manner
            foreach (var thread in threads)
                thread.Abort();
        }

        public void CreateMtDaemonAndAddItToSomeCollections(KohonenCore<StateVector> kohonen, QLearning<KohonenAiState, AI.Action> qLearning, string map, string level, int mapNumber, List<Thread> threads, List<MtSingleDaemon> daemons, List<BlockingCollection<KohonenUpdate>> kohonenUpdateQueues, bool useCosDist = false)
        {
            var queue = new BlockingCollection<KohonenUpdate>(QUEUE_MAX_CAPACTITY);
            var mapSingleDaemon = new MtSingleDaemon(kohonen, qLearning, queue, map, level, 0, HeuristicActive, useCosDist, mapNumber) { IterationStartLearning = ITERATION_OF_SINGLE_THREAD_START_LEARNING };
            threads.Add(new Thread(mapSingleDaemon.ProcessLearning) { IsBackground = true });
            daemons.Add(mapSingleDaemon);
            kohonenUpdateQueues.Add(queue);
        }

        public KeyValuePair<string, string> getMapAndLevel(string mapName)
        {
            switch (mapName)
            {
                case "Map":
                    return new KeyValuePair<string, string>(Properties.Resources.Map, Properties.Resources.Levels);
                case "Map1":
                    return new KeyValuePair<string, string>(Properties.Resources.Map1, Properties.Resources.Levels1);
                case "Map2":
                    return new KeyValuePair<string, string>(Properties.Resources.Map2, Properties.Resources.Levels2);
                case "Map3":
                    return new KeyValuePair<string, string>(Properties.Resources.Map3, Properties.Resources.Levels3);
                case "Map4":
                    return new KeyValuePair<string, string>(Properties.Resources.Map4, Properties.Resources.Levels4);
                case "Map5":
                    return new KeyValuePair<string, string>(Properties.Resources.Map5, Properties.Resources.Levels5);
                default:
                    return new KeyValuePair<string, string>(null, null);
            }
        }

        public int getnumberOfMap(string mapName)
        {
            switch (mapName)
            {
                case "Map":
                    return 0;
                case "Map1":
                    return 1;
                case "Map2":
                    return 2;
                case "Map3":
                    return 3;
                case "Map4":
                    return 4;
                case "Map5":
                    return 5;
                default:
                    return 0;
            }
        }
    }
}
