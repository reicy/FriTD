using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manager.MTCore.Core;
using Manager.Utils;

namespace TDExperimentLib.Experiments
{
    public class Experiment06 : ExperimentBase
    {
        private MtManager manager = new MtManager();
        private Settings s = new Settings();
        private string csvFilename = "results_qLearningTests.csv";
        private string csvFilenameFinal = "results_qLearningTests_final.csv";

        public Experiment06() : base("Exp")
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
            string postfix = "";

            for (int i = 1; i < 20; i++)
            {
                prefix = "run" + i + "_";
                Run("Map", prefix: prefix, postfix: postfix);
                Run("Map1", prefix: prefix, postfix: postfix);
                Run("Map2", prefix: prefix, postfix: postfix);
                Run("Map3", prefix: prefix, postfix: postfix);
                Run("Map4", prefix: prefix, postfix: postfix);
                Run("Map5", prefix: prefix, postfix: postfix);
            }


        }

        private void Run(string map, int numOfThreads = 4, string prefix = "", string postfix = "",
            int iterationsPerThread = 5000, int numberOfQlearnings = 5)
        {
            CustomLogger.LogToFile("===== STARTING " + prefix + map + "_" + postfix + " =====", csvFilename);
            var kohonenFile = InitLearnKohonenPhase(map, numOfThreads, prefix, postfix, iterationsPerThread);
            for (int i = 1; i <= numberOfQlearnings; i++)
            {
                LearnQLearningPhase(map, kohonenFile, numOfThreads, prefix + "q" + i + "/" + numberOfQlearnings + "_", postfix, iterationsPerThread);
            }
            CustomLogger.LogToFile("===== ENDING " + prefix + map + "_" + postfix + " =====", csvFilename);
            CustomLogger.LogToFile("", csvFilename);
            CustomLogger.LogToFile("", csvFilenameFinal);
        }

        private void LearnQLearningPhase(string map, string kohonenFileName, int numOfThreads = 4, string prefix = "", string postfix = "",  int iterationsPerThread = 5000)
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
            s.kohonenLoadFile = kohonenFileName;
            s.qLearningLoadFile = null;

            CustomLogger.filename = prefix + map + "_stats_" + postfix + ".txt";
            CustomLogger.ClearActualLogFile();

            s.numberOfIterationsPerMap = iterationsPerThread;
            s.numberOfIterationsPerMapWithKohonen = 0;

            // first run
            CustomLogger.Log("############### Run 1/4: " + prefix + map + "_" + postfix + " ###############");
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCsvString(), csvFilename);

            s.kohonenLoadFile = s.kohonenSaveFile;
            s.qLearningLoadFile = s.qLearningSaveFile;

            // second run
            CustomLogger.Log("############### Run 2/4: " + prefix + map + "_" + postfix + " ###############");
            s.qLearningLearningRate = 0.3;
            s.qLearningRandomActionProbability = 0.2;
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCsvString(), csvFilename);

            // third run
            CustomLogger.Log("############### Run 3/4: " + prefix + map + "_" + postfix + " ###############");
            s.qLearningLearningRate = 0.2;
            s.qLearningRandomActionProbability = 0.1;
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCsvString(), csvFilename);

            // fourth run - final with fixed kohonen and qlearning - runs only half iterations
            CustomLogger.Log("############### Run 4/4: " + prefix + map + "_" + postfix + " ###############");
            s.numberOfIterationsPerMap = s.numberOfIterationsPerMap/2;
            s.qLearningRandomActionProbability = 0.0001;
            s.qLearningLearningRate = 0.01;
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCsvString(), csvFilename);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCsvString(), csvFilenameFinal);

            CustomLogger.LogToFile("", csvFilename);
        }

        private string InitLearnKohonenPhase(string map, int numOfThreads = 4, string prefix = "", string postfix = "", int iterationsPerThread = 5000)
        {
            // initial settings
            s.ResetToDefault();
            s.maps.Clear();
            for (int i = 0; i < numOfThreads; i++)
            {
                s.maps.Add(map);
            }
            
            s.kohonenSaveFile = prefix + map + "_koh_" + postfix + "_init.txt";
            s.qLearningSaveFile = prefix + map + "_q_" + postfix + "_init.txt";

            s.kohonenLoadFile = null;
            s.qLearningLoadFile = null;

            CustomLogger.filename = prefix + map + "_stats_" + postfix + ".txt";
            CustomLogger.ClearActualLogFile();

            s.numberOfIterationsPerMap = iterationsPerThread;
            s.numberOfIterationsPerMapWithKohonen = s.numberOfIterationsPerMap;

            CustomLogger.LogToFile("=== INIT PHASE - START ===", csvFilename);
            // first run
            CustomLogger.Log("############### Init Run 1/3: " + prefix + map + "_" + postfix + " ###############");
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCsvString(), csvFilename);

            s.kohonenLoadFile = s.kohonenSaveFile;
            s.qLearningLoadFile = s.qLearningSaveFile;

            // second run
            CustomLogger.Log("############### Init Run 2/3: " + prefix + map + "_" + postfix + " ###############");
            s.kohonenLearningRate = 0.25;
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCsvString(), csvFilename);

            // third run
            CustomLogger.Log("############### Init Run 3/3: " + prefix + map + "_" + postfix + " ###############");
            s.qLearningLearningRate = 0.3;
            s.qLearningRandomActionProbability = 0.2;
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCsvString(), csvFilename);

            // fourth run - final with fixed kohonen and qlearning - runs only third iterations
            CustomLogger.Log("############### Init Run 4/3 (just to see what qlearning learned): " + prefix + map + "_" + postfix + " ###############");
            s.numberOfIterationsPerMapWithKohonen = 0;
            s.numberOfIterationsPerMap = s.numberOfIterationsPerMap / 3;
            s.qLearningRandomActionProbability = 0.0001;
            s.qLearningLearningRate = 0.01;
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCsvString(), csvFilename);

            CustomLogger.LogToFile("=== INIT PHASE - END ===", csvFilename);
            CustomLogger.LogToFile("", csvFilename);

            return s.kohonenSaveFile;
        }
    }
}
