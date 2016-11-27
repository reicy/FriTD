using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Manager.Kohonen;
using Manager.MTCore.Core;
using Manager.QLearning;

namespace Manager.MTCore
{
    public class MtManager
    {
        private bool _loadKohonen = false;
        private string _kohonenLoadPath = @"C:\Users\Tomas\Desktop\kohonenLearnt\M010KE.txt"; 
          // @"C:\Users\Tomas\Desktop\kohonenLearnt\cosFixed5k.txt";
        // @"C:\Users\Tomas\Desktop\kohonenLearnt\k24kForM1.txt";

        // @"C:\Users\Tomas\Desktop\kohonenLearnt\k20kCosM0.txt";// @"C:\Users\Tomas\Desktop\kohonenLearnt\Map0K20k.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\k20kHeuriM0.txt"; // @"C:\Users\Tomas\Desktop\kohonenLearnt\CTest.txt";////@"C:\Users\Tomas\Desktop\kohonenLearnt\k24kForM1.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\MixedKohonen24_4_2.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\KMixed24_M1_4_M0.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\k24kForM1.txt";// @"C:\Users\Tomas\Desktop\kohonenLearnt\K3kForMMEfekt2.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\Kohonen3kForM0MEfekt.txt";
        private bool nonEmptyModeCohonenActive = false;


        private int _numOfType2 = 0;
        private const int ThreadsNum = 6;
        private const int Iterations = 10000;
        private const int IterationStopKohonenUpdate = 30000;
        private const int IterationOfSingleThreadStartLearning = 5;
        private const int QueueMaxCapactity = 1;

        private static readonly bool CosDistActive = false;
        private static readonly bool HeuristicActive = true;


        public void ProcessLearning()
        {

            for (var i = 0; i < 10; i++)
                SingleRun();


            //init core
            var qLearning = new QLearning<KohonenAiState>(0.3, 1, 0.5);
            
            var kohonen = new KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5, nonEmptyModeCohonenActive);
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
            var counter = 0;

            Console.WriteLine(DateTime.Now.ToString());

            //init threads
            MtSingleDaemon singleDaemon;
            for (var i = 0; i < ThreadsNum; i++)
            {
                var queue = new BlockingCollection<KohonenUpdate>(QueueMaxCapactity);
                if (i < _numOfType2)
                {
                    Console.WriteLine("map1");
                    singleDaemon = new MtSingleDaemon(kohonen, qLearning, queue, Properties.Resources.Map1, Properties.Resources.Levels1,1, HeuristicActive, CosDistActive)
                    {
                        IterationStartLearning = IterationOfSingleThreadStartLearning
                    };
                }
                else
                {
                    Console.WriteLine("map");
                    singleDaemon = new MtSingleDaemon(kohonen, qLearning, queue, Properties.Resources.Map, Properties.Resources.Levels,0, HeuristicActive, CosDistActive)
                    {
                        IterationStartLearning = IterationOfSingleThreadStartLearning
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
                while (!thread.IsAlive);
            }

            //init stopwatch
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            //Core cycle
            //execute level -> update kohonen -> repeat
            for (var i = 0; i < Iterations; i++)
            {
                
                won = 0;
                lost = 0;
                
                //collect kohonen updates
                KohonenUpdate up;
                
                foreach (var queue in kohonenUpdateQueues)
                {
                    
                    up = queue.Take();
                    kohonenUpdatesToProcess.Add(up);
                    
                }

                //wait for threads to stop
                foreach (var queue in kohonenUpdateQueues)
                    while (queue.Count != QueueMaxCapactity) ;

                //update kohonen
                foreach (var update in kohonenUpdatesToProcess)
                    if (IterationStopKohonenUpdate > i) kohonen.ReArrange(update.Row, update.Col, update.Vector);
                if (IterationStopKohonenUpdate == i)
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
            var elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",ts.Hours, ts.Minutes, ts.Seconds,ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
            MtStats.PrintLevelsOfEnding();
            MtStats.PrintTotalScore();

            kohonen.PrintAccesses();
            kohonen.Displ();
            kohonen.Displ(11);
            
            //  Console.WriteLine("won: " + won + " lost: " + lost);

            Console.WriteLine(DateTime.Now.ToString());
            qLearning.QValDisp();

            //Dispose threads in inhuman manner, TODO change to more human approach
            foreach (var thread in threads)
                thread.Abort();
        }





        public void SingleRun()
        {



            //init core
            var qLearning = new QLearning<KohonenAiState>(0.3, 1, 0.5);

            var kohonen = new KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5, nonEmptyModeCohonenActive);
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
            var counter = 0;

            Console.WriteLine(DateTime.Now.ToString());

            //init threads
            MtSingleDaemon singleDaemon;
            for (var i = 0; i < ThreadsNum; i++)
            {
                var queue = new BlockingCollection<KohonenUpdate>(QueueMaxCapactity);
                if (i < _numOfType2)
                {
                    Console.WriteLine("map1");
                    singleDaemon = new MtSingleDaemon(kohonen, qLearning, queue, Properties.Resources.Map1, Properties.Resources.Levels1, 1, HeuristicActive, CosDistActive)
                    {
                        IterationStartLearning = IterationOfSingleThreadStartLearning
                    };
                }
                else
                {
                    Console.WriteLine("map");
                    singleDaemon = new MtSingleDaemon(kohonen, qLearning, queue, Properties.Resources.Map, Properties.Resources.Levels, 0, HeuristicActive, CosDistActive)
                    {
                        IterationStartLearning = IterationOfSingleThreadStartLearning
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
                while (!thread.IsAlive) ;
            }

            //init stopwatch
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            //Core cycle
            //execute level -> update kohonen -> repeat
            for (var i = 0; i < Iterations; i++)
            {

                won = 0;
                lost = 0;

                //collect kohonen updates
                KohonenUpdate up;

                foreach (var queue in kohonenUpdateQueues)
                {

                    up = queue.Take();
                    kohonenUpdatesToProcess.Add(up);

                }

                //wait for threads to stop
                foreach (var queue in kohonenUpdateQueues)
                    while (queue.Count != QueueMaxCapactity) ;

                //update kohonen
                foreach (var update in kohonenUpdatesToProcess)
                    if (IterationStopKohonenUpdate > i) kohonen.ReArrange(update.Row, update.Col, update.Vector);
                if (IterationStopKohonenUpdate == i)
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
            var elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
            MtStats.PrintLevelsOfEnding();
            MtStats.PrintTotalScore();

       

            //Dispose threads in inhuman manner
            foreach (var thread in threads)
                thread.Abort();
        }

        // run 6 types of maps from one kohonen and qlearning
        public void ExperimentRun1()
        {
            /*
            TODO: interationstart learning in MTSingleDaemon
            */

            // create qlearning and kohonen and init other very important things...

            var qLearning = new QLearning<KohonenAiState>(0.3, 1, 0.5);
            var kohonen = new KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5, nonEmptyModeCohonenActive);
            var weight = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            kohonen.SetWeight(weight);
            var threads = new List<Thread>();
            var daemons = new List<MtSingleDaemon>();
            var kohonenUpdateQueues = new List<BlockingCollection<KohonenUpdate>>();
            var kohonenUpdatesToProcess = new List<KohonenUpdate>();

            // init threads
            CreateMTDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map,  Properties.Resources.Levels,  0, threads, daemons, kohonenUpdateQueues);
            CreateMTDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map1, Properties.Resources.Levels1, 1, threads, daemons, kohonenUpdateQueues);
            CreateMTDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map2, Properties.Resources.Levels2, 2, threads, daemons, kohonenUpdateQueues);
            CreateMTDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map3, Properties.Resources.Levels3, 3, threads, daemons, kohonenUpdateQueues);
            CreateMTDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map4, Properties.Resources.Levels4, 4, threads, daemons, kohonenUpdateQueues);
            CreateMTDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map5, Properties.Resources.Levels5, 5, threads, daemons, kohonenUpdateQueues);

            Console.WriteLine("\n\n ########################## \nSTARTING 6 MAPS in 6 THREADS WITH KOHONEN LEARNING TURNED ON\nSettings: QLearning<KohonenAiState>(0.3, 1, 0.5); KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5, nonEmptyModeCohonenActive); \n ########################## ");
            StartMAGIC(kohonen, 5000, 5001, threads, daemons, kohonenUpdateQueues, kohonenUpdatesToProcess);
    
            MtStats.PrintLevelsOfEnding();
            MtStats.PrintTotalScore();
            MtStats.ResetAll();

            qLearning.Epsilon = 0.1;
            threads.Clear();
            daemons.Clear();
            kohonenUpdateQueues.Clear();

            CreateMTDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map,  Properties.Resources.Levels,  0, threads, daemons, kohonenUpdateQueues);
            CreateMTDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map1, Properties.Resources.Levels1, 1, threads, daemons, kohonenUpdateQueues);
            CreateMTDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map2, Properties.Resources.Levels2, 2, threads, daemons, kohonenUpdateQueues);
            CreateMTDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map3, Properties.Resources.Levels3, 3, threads, daemons, kohonenUpdateQueues);
            CreateMTDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map4, Properties.Resources.Levels4, 4, threads, daemons, kohonenUpdateQueues);
            CreateMTDaemonAndAddItToSomeCollections(kohonen, qLearning, Properties.Resources.Map5, Properties.Resources.Levels5, 5, threads, daemons, kohonenUpdateQueues);

            Console.WriteLine("\n\n ########################## \nSTARTING 6 MAPS in 6 THREADS WITH KOHONEN LEARNING TURNED OFF\nSettings: QLearning<KohonenAiState>(0.1, 1, 0.5); KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5, nonEmptyModeCohonenActive); \n ########################## ");
            StartMAGIC(kohonen, 5000, 0, threads, daemons, kohonenUpdateQueues, kohonenUpdatesToProcess);

            MtStats.PrintLevelsOfEnding();
            MtStats.PrintTotalScore();
        }

        public void StartMAGIC(KohonenCore<StateVector> kohonen, int numOfIterationsPerThread, int numOfIterationsWithKohonenLearningPerThread, List<Thread> threads, List<MtSingleDaemon> daemons, List<BlockingCollection<KohonenUpdate>> kohonenUpdateQueues, List<KohonenUpdate> kohonenUpdatesToProcess)
        {
            var won = 0;
            var lost = 0;
            var counter = 0;

            //start threads
            foreach (var thread in threads)
            {
                thread.Start();
                while (!thread.IsAlive) ;
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
                KohonenUpdate up;

                foreach (var queue in kohonenUpdateQueues)
                {

                    up = queue.Take();
                    kohonenUpdatesToProcess.Add(up);

                }

                //wait for threads to stop
                foreach (var queue in kohonenUpdateQueues)
                    while (queue.Count != QueueMaxCapactity) ;

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
            var elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            //Dispose threads in inhuman manner
            foreach (var thread in threads)
                thread.Abort();
        }

        public void CreateMTDaemonAndAddItToSomeCollections(KohonenCore<StateVector> kohonen, QLearning<KohonenAiState> qLearning, string map, string level, int mapNumber, List<Thread> threads, List<MtSingleDaemon> daemons, List<BlockingCollection<KohonenUpdate>> kohonenUpdateQueues)
        {
            var queue = new BlockingCollection<KohonenUpdate>(QueueMaxCapactity);
            var mapSingleDaemon = new MtSingleDaemon(kohonen, qLearning, queue, map, level, 0, HeuristicActive, CosDistActive, mapNumber) { IterationStartLearning = IterationOfSingleThreadStartLearning };
            threads.Add(new Thread(mapSingleDaemon.ProcessLearning));
            daemons.Add(mapSingleDaemon);
            kohonenUpdateQueues.Add(queue);
        }

    }
}
