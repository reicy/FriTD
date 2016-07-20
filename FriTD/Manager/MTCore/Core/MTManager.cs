﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private bool _loadKohonen = false;
        private string _kohonenLoadPath = @"C:\Users\Tomas\Desktop\kohonenLearnt\k24kForM1.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\MixedKohonen24_4_2.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\KMixed24_M1_4_M0.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\k24kForM1.txt";// @"C:\Users\Tomas\Desktop\kohonenLearnt\K3kForMMEfekt2.txt";//@"C:\Users\Tomas\Desktop\kohonenLearnt\Kohonen3kForM0MEfekt.txt";
        private bool nonEmptyModeCohonenActive = true;


        private int _numOfType2 = 0;
        private const int ThreadsNum = 4;
        private const int Iterations = 5000;
        private const int IterationStopKohonenUpdate = 3000;
        private const int IterationOfSingleThreadStartLearning = 5;
        private const int QueueMaxCapactity = 1;
        



        public void ProcessLearning()
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
            var weight = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1000000, 100, 0, 1, 1, 1, 1, 1, 1, 1, 1 };

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
                    singleDaemon = new MtSingleDaemon(kohonen, qLearning, queue, Properties.Resources.Map1, Properties.Resources.Levels1,1)
                    {
                        IterationStartLearning = IterationOfSingleThreadStartLearning
                    };
                }
                else
                {
                    Console.WriteLine("map");
                    singleDaemon = new MtSingleDaemon(kohonen, qLearning, queue, Properties.Resources.Map, Properties.Resources.Levels,0)
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
            kohonen.PrintAccesses();
            kohonen.Displ();
            kohonen.Displ(11);
            MtStats.PrintLevelsOfEnding();
            //  Console.WriteLine("won: " + won + " lost: " + lost);

            Console.WriteLine(DateTime.Now.ToString());
            qLearning.QValDisp();

            //Dispose threads in inhuman manner, TODO change to more human approach
            foreach (var thread in threads)
            {
                thread.Abort();
            }

        }

        
    }
}
