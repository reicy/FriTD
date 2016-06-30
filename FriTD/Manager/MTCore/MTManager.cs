using System;
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

        private const int ThreadsNum = 4;
        private const int Iterations = 10000;
        private const int IterationStopKohonenUpdate = 2000;
        private const int QueueMaxCapactity = 1;
        



        public void ProcessLearning()
        {
           

            //init core
            var qLearning = new QLearning<KohonenAiState>(0.2, 1, 0.5);
            var kohonen = new KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5);
            List<Thread> threads = new List<Thread>();
            List<MtSingleDaemon> daemons = new List<MtSingleDaemon>();
            List< BlockingCollection <KohonenUpdate>> kohonenUpdateQueues = new List<BlockingCollection<KohonenUpdate>>();
            List<KohonenUpdate> kohonenUpdatesToProcess = new List<KohonenUpdate>();

            int won = 0;
            int lost = 0;
            int counter = 0;

            Console.WriteLine(DateTime.Now.ToString());

            //init threads
            for (int i = 0; i < ThreadsNum; i++)
            {
                var queue = new BlockingCollection<KohonenUpdate>(QueueMaxCapactity);
                var singleDaemon = new MtSingleDaemon(kohonen, qLearning, queue) { IterationStartLearning  = 50};
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
                kohonenUpdatesToProcess.Clear();
                
                foreach (var mtSingleDaemon in daemons)
                {

                    won += mtSingleDaemon.Won;
                    lost += mtSingleDaemon.Lost;
                   
                }
               
                
            }
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
