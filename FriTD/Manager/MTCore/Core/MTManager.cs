using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Manager.AI;
using Manager.GameStates;
using Manager.Kohonen;

using Manager.QLearning;

namespace Manager.MTCore
{
    public class MtManager
    {
        private bool _loadKohonen = true;
        private string _kohonenLoadPath = @"C:\Users\Tomas\Desktop\kohonenLearnt\M010KE.txt"; 
          // @"C:\Users\Tomas\Desktop\kohonenLearnt\cosFixed5k.txt";
        // @"C:\Users\Tomas\Desktop\kohonenLearnt\k24kForM1.txt";

        // @"C:\Users\Tomas\Desktop\kohonenLearnt\k20kCosM0.txt";// @"C:\Users\Tomas\Desktop\kohonenLearnt\Map0K20k.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\k20kHeuriM0.txt"; // @"C:\Users\Tomas\Desktop\kohonenLearnt\CTest.txt";////@"C:\Users\Tomas\Desktop\kohonenLearnt\k24kForM1.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\MixedKohonen24_4_2.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\KMixed24_M1_4_M0.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\k24kForM1.txt";// @"C:\Users\Tomas\Desktop\kohonenLearnt\K3kForMMEfekt2.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\Kohonen3kForM0MEfekt.txt";
        private bool nonEmptyModeCohonenActive = false;


        private int _numOfType2 = 0;
        private const int ThreadsNum = 4;
        private const int Iterations = 1000;
        private const int IterationStopKohonenUpdate = 0;
        private const int IterationOfSingleThreadStartLearning = 5;
        private const int QueueMaxCapactity = 1;

        private static readonly bool CosDistActive = false;
        private static readonly bool HeuristicActive = true;


        public void ProcessLearning()
        {

            for (int i = 0; i < 10; i++)
            {
                SingleRun();
            }
           


            //init core
            var qLearning = new QLearning<KohonenAiState>(0.3, 1, 0.5);
            
            var kohonen = new KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5, nonEmptyModeCohonenActive);
            //load kohonen
            if (_loadKohonen)
            {
                kohonen.Load(_kohonenLoadPath);
               // kohonen.Displ();
            }

            //var weight = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 100, 0.001, 1000, 1, 1, 1, 1, 1, 1, 1, 1};
            //var weight = new double[] { 1000000, 1000000, 1000000, 10000, 10000, 10000, 100, 100, 1, 1, 0.001, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            //var weight = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1000000, 100, 0, 1, 1, 1, 1, 1, 1, 1, 1 };
            var weight = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };


            kohonen.SetWeight(weight);
            List<Thread> threads = new List<Thread>();
            List<MtSingleDaemon> daemons = new List<MtSingleDaemon>();
            List< BlockingCollection <KohonenUpdate>> kohonenUpdateQueues = new List<BlockingCollection<KohonenUpdate>>();
            List<KohonenUpdate> kohonenUpdatesToProcess = new List<KohonenUpdate>();

            int won = 0;
            int lost = 0;
            int counter = 0;

            Console.WriteLine(DateTime.Now.ToString());

            //init threads
            MtSingleDaemon singleDaemon;
            for (int i = 0; i < ThreadsNum; i++)
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
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //Core cycle
            //execute level -> update kohonen -> repeat
            for (int i = 0; i < Iterations; i++)
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
                {
                    while (queue.Count != QueueMaxCapactity) ;
                }

                //update kohonen
                foreach (var update in kohonenUpdatesToProcess)
                {
                    if (IterationStopKohonenUpdate > i) kohonen.ReArrange(update.Row, update.Col, update.Vector);
                }
                if (IterationStopKohonenUpdate == i)
                {
                    kohonen.ResetAccesses();
                }
                kohonenUpdatesToProcess.Clear();
                
                foreach (var mtSingleDaemon in daemons)
                {

                    won += mtSingleDaemon.Won;
                    lost += mtSingleDaemon.Lost;
                   
                }
               
                
            }

            // stop watch and displ time elapsed
          
            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",ts.Hours, ts.Minutes, ts.Seconds,ts.Milliseconds / 10);
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
            {
                thread.Abort();
            }

        }





        public void SingleRun()
        {



            //init core
            var qLearning = new QLearning<KohonenAiState>(0.3, 1, 0.5);

            var kohonen = new KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5, nonEmptyModeCohonenActive);
            //load kohonen
            if (_loadKohonen)
            {
                kohonen.Load(_kohonenLoadPath);
                // kohonen.Displ();
            }

            //var weight = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 100, 0.001, 1000, 1, 1, 1, 1, 1, 1, 1, 1};
            //var weight = new double[] { 1000000, 1000000, 1000000, 10000, 10000, 10000, 100, 100, 1, 1, 0.001, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            //var weight = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1000000, 100, 0, 1, 1, 1, 1, 1, 1, 1, 1 };
            var weight = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };


            kohonen.SetWeight(weight);
            List<Thread> threads = new List<Thread>();
            List<MtSingleDaemon> daemons = new List<MtSingleDaemon>();
            List<BlockingCollection<KohonenUpdate>> kohonenUpdateQueues = new List<BlockingCollection<KohonenUpdate>>();
            List<KohonenUpdate> kohonenUpdatesToProcess = new List<KohonenUpdate>();

            int won = 0;
            int lost = 0;
            int counter = 0;

            Console.WriteLine(DateTime.Now.ToString());

            //init threads
            MtSingleDaemon singleDaemon;
            for (int i = 0; i < ThreadsNum; i++)
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
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //Core cycle
            //execute level -> update kohonen -> repeat
            for (int i = 0; i < Iterations; i++)
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
                {
                    while (queue.Count != QueueMaxCapactity) ;
                }

                //update kohonen
                foreach (var update in kohonenUpdatesToProcess)
                {
                    if (IterationStopKohonenUpdate > i) kohonen.ReArrange(update.Row, update.Col, update.Vector);
                }
                if (IterationStopKohonenUpdate == i)
                {
                    kohonen.ResetAccesses();
                }
                kohonenUpdatesToProcess.Clear();

                foreach (var mtSingleDaemon in daemons)
                {

                    won += mtSingleDaemon.Won;
                    lost += mtSingleDaemon.Lost;

                }


            }

            // stop watch and displ time elapsed

            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
            MtStats.PrintLevelsOfEnding();
            MtStats.PrintTotalScore();

       

            //Dispose threads in inhuman manner
            foreach (var thread in threads)
            {
                thread.Abort();
            }
        }

        public void ExperimentRun()
        {
            /*
            TODO: interation start learning in MTSingleDaemon
            */

            // create qlearning and kohonen and init other very important things...
            Console.WriteLine(DateTime.Now.ToString() + ": SETTING UP THINGS");

            var qLearning = new QLearning<KohonenAiState>(0.3, 1, 0.5);
            var kohonen = new KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5, nonEmptyModeCohonenActive);
            var weight = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            kohonen.SetWeight(weight);
            List<Thread> threads = new List<Thread>();
            List<MtSingleDaemon> daemons = new List<MtSingleDaemon>();
            List<BlockingCollection<KohonenUpdate>> kohonenUpdateQueues = new List<BlockingCollection<KohonenUpdate>>();
            List<KohonenUpdate> kohonenUpdatesToProcess = new List<KohonenUpdate>();

            int won = 0;
            int lost = 0;
            int counter = 0;


            // heat up kohonen and qlearning - play every map 300 times

            // lower learning/exploration rate of both kohonen and qlearning

            // run every map few hundred times

            // save score


            ////////////////////////////////////////////
            
            //init threads
            for (int i = 0; i < ThreadsNum; i++)
            {
                MtSingleDaemon singleDaemon;
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
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //Core cycle
            //execute level -> update kohonen -> repeat
            for (int i = 0; i < Iterations; i++)
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
                {
                    while (queue.Count != QueueMaxCapactity) ;
                }

                //update kohonen
                foreach (var update in kohonenUpdatesToProcess)
                {
                    if (IterationStopKohonenUpdate > i) kohonen.ReArrange(update.Row, update.Col, update.Vector);
                }
                if (IterationStopKohonenUpdate == i)
                {
                    kohonen.ResetAccesses();
                }
                kohonenUpdatesToProcess.Clear();

                foreach (var mtSingleDaemon in daemons)
                {

                    won += mtSingleDaemon.Won;
                    lost += mtSingleDaemon.Lost;

                }


            }

            // stop watch and displ time elapsed

            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
            MtStats.PrintLevelsOfEnding();
            MtStats.PrintTotalScore();



            //Dispose threads in inhuman manner
            foreach (var thread in threads)
            {
                thread.Abort();
            }
        }

    }
}
