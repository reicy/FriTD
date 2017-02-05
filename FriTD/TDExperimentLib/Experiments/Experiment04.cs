using System.Collections.Generic;
using Manager.MTCore.Core;
using Manager.Utils;

namespace TDExperimentLib.Experiments
{
    public class Experiment04 : ExperimentBase
    {
        private MtManager manager = new MtManager();

        private List<string> maps = new List<string>();

        private int numberOfIterationsPerMap = 5000;
        private int numberOfIterationsPerMapWithKohonen = 5000;
        private bool useCosDist = false;
        private double qLearningRandomActionProbability = 0.3;
        private double qLearningDiscountFactor = 1;
        private double qLearningLearningRate = 0.5;
        private int kohonenRows = 30;
        private int kohonenCols = 30;
        private double kohonenRadius = 2;
        private double kohonenLearningRate = 0.5;
        private double kohonenDistFactor = 1;
        private string kohonenLoadFile = null;
        private string qLearningLoadFile = null;
        private string kohonenSaveFile = null;
        private string qLearningSaveFile = null;

        public Experiment04() : base("Table")
        {
        }

        public override void Run()
        {

            CustomLogger.logToFile = true;

            var prefix = "6_";
            RunMap("Map1", prefix: prefix);
            RunMap("Map2", prefix: prefix);
            RunMap("Map3", prefix: prefix);
            RunMap("Map4", prefix: prefix);
            RunMap("Map5", prefix: prefix);

            prefix = "7_";
            RunMap("Map", prefix: prefix);
            RunMap("Map1", prefix: prefix);
            RunMap("Map2", prefix: prefix);
            RunMap("Map3", prefix: prefix);
            RunMap("Map4", prefix: prefix);
            RunMap("Map5", prefix: prefix);

            prefix = "8_";
            RunMap("Map", prefix: prefix);
            RunMap("Map1", prefix: prefix);
            RunMap("Map2", prefix: prefix);
            RunMap("Map3", prefix: prefix);
            RunMap("Map4", prefix: prefix);
            RunMap("Map5", prefix: prefix);

            prefix = "9_";
            RunMap("Map", prefix: prefix);
            RunMap("Map1", prefix: prefix);
            RunMap("Map2", prefix: prefix);
            RunMap("Map3", prefix: prefix);
            RunMap("Map4", prefix: prefix);
            RunMap("Map5", prefix: prefix);
        }

        private void RunMap(string map, int numOfThreads = 4, string prefix = "")
        {
            SetDefaultValues();
            maps.Clear();
            for (int i = 0; i < numOfThreads; i++)
            {
                maps.Add(map);
            }

            kohonenSaveFile = prefix + map + "_koh.txt";
            qLearningSaveFile = prefix + map + "_q.txt";
            kohonenLoadFile = null;
            qLearningLoadFile = null;

            CustomLogger.filename = prefix + map + "_stats.txt";
            CustomLogger.ClearActualLogFile();

            numberOfIterationsPerMap = 3000;
            numberOfIterationsPerMapWithKohonen = 3000;
            kohonenRadius = 3;
            RunExperiment();

            kohonenLoadFile = kohonenSaveFile;
            qLearningLoadFile = qLearningSaveFile;

            numberOfIterationsPerMap = 3000;
            numberOfIterationsPerMapWithKohonen = 3000;
            kohonenLearningRate = 0.3;
            kohonenRadius = 2;
            RunExperiment();

            numberOfIterationsPerMap = 3000;
            numberOfIterationsPerMapWithKohonen = 3000;
            kohonenLearningRate = 0.15;
            RunExperiment();

            numberOfIterationsPerMap = 3000;
            numberOfIterationsPerMapWithKohonen = 0;
            RunExperiment();

            numberOfIterationsPerMap = 3000;
            numberOfIterationsPerMapWithKohonen = 0;
            qLearningRandomActionProbability = 0.2;
            qLearningLearningRate = 0.35;
            RunExperiment();

            numberOfIterationsPerMap = 3000;
            numberOfIterationsPerMapWithKohonen = 0;
            qLearningRandomActionProbability = 0.1;
            RunExperiment();

            numberOfIterationsPerMap = 3000;
            numberOfIterationsPerMapWithKohonen = 0;
            qLearningRandomActionProbability = 0.001;
            qLearningLearningRate = 0.01;
            RunExperiment();
        }

        public void RunExperiment()
        {
            manager.ExperimentRun(maps, numberOfIterationsPerMap, numberOfIterationsPerMapWithKohonen, useCosDist,
                qLearningRandomActionProbability, qLearningDiscountFactor, qLearningLearningRate,
                kohonenRows, kohonenCols, kohonenRadius, kohonenLearningRate, kohonenDistFactor, kohonenLoadFile,
                qLearningLoadFile, kohonenSaveFile, qLearningSaveFile);
        }

        private void SetDefaultValues()
        {
            numberOfIterationsPerMap = 5000;
            numberOfIterationsPerMapWithKohonen = 5000;
            useCosDist = false;
            qLearningRandomActionProbability = 0.3;
            qLearningDiscountFactor = 1;
            qLearningLearningRate = 0.5;
            kohonenRows = 30;
            kohonenCols = 30;
            kohonenRadius = 2;
            kohonenLearningRate = 0.5;
            kohonenDistFactor = 1;
            kohonenLoadFile = null;
            qLearningLoadFile = null;
            kohonenSaveFile = null;
            qLearningSaveFile = null;
        }
    }
}
