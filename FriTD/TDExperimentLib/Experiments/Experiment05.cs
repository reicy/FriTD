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
        private string csvFilename = "results.csv";

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

            postfix = "withKohonen";
            prefix = "run1_";
            withKohonen = true;
            RunMap("Map", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map1", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map2", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map3", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map4", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map5", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            postfix = "withoutKohonen";
            withKohonen = false;
            RunMap("Map", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map1", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map2", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map3", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map4", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map5", prefix: prefix, postfix: postfix, withKohonen: withKohonen);

            postfix = "withKohonen";
            prefix = "run2_";
            withKohonen = true;
            RunMap("Map", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map1", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map2", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map3", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map4", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map5", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            postfix = "withoutKohonen";
            withKohonen = false;
            RunMap("Map", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map1", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map2", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map3", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map4", prefix: prefix, postfix: postfix, withKohonen: withKohonen);
            RunMap("Map5", prefix: prefix, postfix: postfix, withKohonen: withKohonen);

        }

        private void RunMap(string map, int numOfThreads = 4, string prefix = "", string postfix = "", bool withKohonen = true, int iterationsPerThread = 5000)
        {
            // initial settings
            s.ResetToDefault();
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
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCVSString(), csvFilename);

            s.kohonenLoadFile = s.kohonenSaveFile;
            s.qLearningLoadFile = s.qLearningSaveFile;

            // second run
            CustomLogger.Log("############### Run 2/5: " + prefix + map + "_" + postfix + " ###############");
            s.kohonenLearningRate = 0.25;
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCVSString(), csvFilename);

            // third run
            CustomLogger.Log("############### Run 3/5: " + prefix + map + "_" + postfix + " ###############");
            if (!withKohonen) s.numberOfIterationsPerMapWithKohonen = 0;
            s.qLearningLearningRate = 0.3;
            s.qLearningRandomActionProbability = 0.2;
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCVSString(), csvFilename);

            // fourth run
            CustomLogger.Log("############### Run 4/5: " + prefix + map + "_" + postfix + " ###############");
            s.qLearningLearningRate = 0.25;
            s.qLearningRandomActionProbability = 0.1;
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCVSString(), csvFilename);

            // fifth run - final with fixed kohonen and qlearning - runs only half iterations
            CustomLogger.Log("############### Run 5/5: " + prefix + map + "_" + postfix + " ###############");
            s.numberOfIterationsPerMapWithKohonen = 0;
            s.numberOfIterationsPerMap = s.numberOfIterationsPerMap/2;
            s.qLearningRandomActionProbability = 0.0001;
            s.qLearningLearningRate = 0.1;
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCVSString(), csvFilename);

            CustomLogger.LogToFile("", csvFilename);
        }
    }
}
