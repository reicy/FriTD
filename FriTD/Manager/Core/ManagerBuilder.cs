using Manager.Kohonen;
using Manager.MTCore;
using Manager.QLearning;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;

namespace Manager.Core
{
    public class ManagerBuilder
    {
        public static Manager BuildPlayerManager()
        {

            return null;
        }

        public static Manager BuildAiLearningManager()
        {
            var mng = new Manager();
            mng.InsertAi();
            return mng;
        }

        public static Manager BuildObservableAiLearningManager(StreamReader reader)
        {
            var mng = new Manager();
            mng.InsertAi(reader);
            return mng;
        }

        public static Manager BuildSimplePlayerManager()
        {
            return new Manager();
        }

        public static MTCore.MtSingleDaemon BuildMTSingleDaemon(int level, string mobs = null)
        {
            string map = null;
            string levels = null;

            switch(level)
            {
                case 0:
                    map = Properties.Resources.Map;
                    levels = Properties.Resources.Levels;
                    break;
                case 1:
                    map = Properties.Resources.Map1;
                    levels = Properties.Resources.Levels1;
                    break;
                case 2:
                    map = Properties.Resources.Map2;
                    levels = Properties.Resources.Levels2;
                    break;
                case 3:
                    map = Properties.Resources.Map3;
                    levels = Properties.Resources.Levels3;
                    break;
                case 4:
                    map = Properties.Resources.Map4;
                    levels = Properties.Resources.Levels4;
                    break;
                case 5:
                    map = Properties.Resources.Map5;
                    levels = Properties.Resources.Levels5;
                    break;
            }

            if(mobs!=null)
            {
                levels = mobs;
            }


            var qLearning = new QLearning<KohonenAiState>(0.3, 1, 0.5);
            var kohonen = new KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5, false);
            List<KohonenUpdate> updatesQueue = new List<KohonenUpdate>();

            return new MTCore.MtSingleDaemon(kohonen, qLearning, updatesQueue, map, levels, 0, true, false)
            {
                IterationStartLearning = 5
            };
        }
    }
}