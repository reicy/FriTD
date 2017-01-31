using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manager.MTCore.Core;
using Manager.Utils;

namespace TDExperimentLib.Experiments
{
    public class Experiment05 : ExperimentBase
    {
        private MtManager manager = new MtManager();
        private Settings s = new Settings();
        private string csvFilename = "results_cosDist.csv";

        public Experiment05() : base("Exp")
        {
        }

        public override KeyValuePair<bool, string> SetData(Dictionary<string, object> propVals)
        {
            return new KeyValuePair<bool, string>(true, "");
        }

        public override void Run()
        {
            CustomLogger.logToFile = true;
            string prefix;
            string postfix;
            bool withKohonen, useCostDist;

            for (int i = 9; i < 30; i++)
            {
                postfix = "withoutKohonen";
                prefix = "run" + i + "_cosDist_";
                useCostDist = true;
                withKohonen = false;
                RunMap("Map", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist);
                RunMap("Map1", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist);
                RunMap("Map2", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist);
                RunMap("Map3", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist);
                RunMap("Map4", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist);
                RunMap("Map5", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist);
                postfix = "withKohonen";
                withKohonen = true;
                RunMap("Map", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist);
                RunMap("Map1", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist);
                RunMap("Map2", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist);
                RunMap("Map3", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist);
                RunMap("Map4", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist);
                RunMap("Map5", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist);
            }


        }

        private void RunMap(string map, int numOfThreads = 4, string prefix = "", string postfix = "",
            bool withKohonen = true, int iterationsPerThread = 5000, bool useCostDist = false)
        {
            // initial settings
            s.ResetToDefault();
            s.useCosDist = useCostDist;
            s.maps.Clear();
            for (int i = 0; i < numOfThreads; i++)
            {
                s.maps.Add(map);
            }

            s.kohonenSaveFile = prefix + map + "_koh_" + postfix + ".txt";
            s.qLearningSaveFile = prefix + map + "_q_" + postfix + ".txt";
            s.kohonenLoadFile = null;
            s.qLearningLoadFile = null;

            CustomLogger.filename = prefix + map + "_stats_" + postfix + ".txt";
            CustomLogger.ClearActualLogFile();

            // first run
            CustomLogger.Log("############### Run 1/5: " + prefix + map + "_" + postfix + " ###############");
            s.numberOfIterationsPerMap = iterationsPerThread;
            s.numberOfIterationsPerMapWithKohonen = s.numberOfIterationsPerMap;
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCsvString(), csvFilename);

            s.kohonenLoadFile = s.kohonenSaveFile;
            s.qLearningLoadFile = s.qLearningSaveFile;

            // second run
            CustomLogger.Log("############### Run 2/5: " + prefix + map + "_" + postfix + " ###############");
            s.kohonenLearningRate = 0.25;
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCsvString(), csvFilename);

            // third run
            CustomLogger.Log("############### Run 3/5: " + prefix + map + "_" + postfix + " ###############");
            if (!withKohonen) s.numberOfIterationsPerMapWithKohonen = 0;
            s.qLearningLearningRate = 0.3;
            s.qLearningRandomActionProbability = 0.2;
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCsvString(), csvFilename);

            // fourth run
            CustomLogger.Log("############### Run 4/5: " + prefix + map + "_" + postfix + " ###############");
            s.qLearningLearningRate = 0.25;
            s.qLearningRandomActionProbability = 0.1;
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCsvString(), csvFilename);

            // fifth run - final with fixed kohonen and qlearning - runs only half iterations
            CustomLogger.Log("############### Run 5/5: " + prefix + map + "_" + postfix + " ###############");
            s.numberOfIterationsPerMapWithKohonen = 0;
            s.numberOfIterationsPerMap = s.numberOfIterationsPerMap/2;
            s.qLearningRandomActionProbability = 0.0001;
            s.qLearningLearningRate = 0.01;
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCsvString(), csvFilename);

            CustomLogger.LogToFile("", csvFilename);
        }
    }
}
