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
        private string csvFilename = "results_kohSize_euclid.csv";
        private string csvFilenameFinal = "results_kohSize_euclid_final.csv";

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
            bool withKohonen;
            bool useCostDist = false;
            int kohonenSize = 15;

            for (int i = 1; i < 10; i++)
            {
                postfix = "withoutKohonen_kohSize" + kohonenSize;
                prefix = "run" + i + "_cosDist_";
                withKohonen = false;
                RunMap("Map", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize:kohonenSize);
                RunMap("Map1", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map2", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map3", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map4", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map5", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                CustomLogger.LogToFile("", csvFilenameFinal);
                postfix = "withKohonen";
                withKohonen = true;
                RunMap("Map", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map1", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map2", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map3", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map4", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map5", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                CustomLogger.LogToFile("", csvFilenameFinal);
            }

            kohonenSize = 60;

            for (int i = 1; i < 10; i++)
            {
                postfix = "withoutKohonen_kohSize" + kohonenSize;
                prefix = "run" + i + "_cosDist_";
                withKohonen = false;
                RunMap("Map", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map1", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map2", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map3", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map4", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map5", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                CustomLogger.LogToFile("", csvFilenameFinal);
                postfix = "withKohonen";
                withKohonen = true;
                RunMap("Map", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map1", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map2", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map3", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map4", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                RunMap("Map5", prefix: prefix, postfix: postfix, withKohonen: withKohonen, useCostDist: useCostDist, kohonenSize: kohonenSize);
                CustomLogger.LogToFile("", csvFilenameFinal);
            }


        }

        private void RunMap(string map, int numOfThreads = 4, string prefix = "", string postfix = "",
            bool withKohonen = true, int iterationsPerThread = 5000, bool useCostDist = false, int kohonenSize = 30)
        {
            // initial settings
            s.ResetToDefault();
            s.kohonenRows = kohonenSize;
            s.kohonenCols = kohonenSize;
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
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCsvString(), csvFilenameFinal);

            CustomLogger.LogToFile("", csvFilename);
        }
    }
}
