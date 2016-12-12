using System;
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

        public override KeyValuePair<bool, string> SetData(Dictionary<string, object> propVals)
        {
            return new KeyValuePair<bool, string>(true, "");
        }

        public override void Run()
        {

            CustomLogger.logToFile = true;

            RunMap("Map");
            RunMap("Map1");
            RunMap("Map2");
            RunMap("Map3");
            RunMap("Map4");
            RunMap("Map5");

        }

        private void RunMap(string map, int numOfThreads = 4)
        {
            SetDefaultValues();
            maps.Clear();
            for (int i = 0; i < numOfThreads; i++)
            {
                maps.Add(map);
            }

            kohonenSaveFile = map + "_koh.txt";
            qLearningSaveFile = map + "_q.txt";
            kohonenLoadFile = null;
            qLearningLoadFile = null;

            CustomLogger.filename = map + "_stats.txt";
            CustomLogger.ClearActualLogFile();

            numberOfIterationsPerMap = 2500;
            numberOfIterationsPerMapWithKohonen = 2500;
            kohonenRadius = 3;
            RunExperiment();

            kohonenLoadFile = kohonenSaveFile;
            qLearningLoadFile = qLearningSaveFile;

            numberOfIterationsPerMap = 2500;
            numberOfIterationsPerMapWithKohonen = 2500;
            kohonenLearningRate = 0.3;
            kohonenRadius = 2;
            RunExperiment();

            numberOfIterationsPerMap = 1500;
            numberOfIterationsPerMapWithKohonen = 1500;
            kohonenLearningRate = 0.15;
            RunExperiment();

            numberOfIterationsPerMap = 2500;
            numberOfIterationsPerMapWithKohonen = 0;
            RunExperiment();

            numberOfIterationsPerMap = 2500;
            numberOfIterationsPerMapWithKohonen = 0;
            qLearningRandomActionProbability = 0.2;
            qLearningLearningRate = 0.35;
            RunExperiment();

            numberOfIterationsPerMap = 2500;
            numberOfIterationsPerMapWithKohonen = 0;
            qLearningRandomActionProbability = 0.1;
            RunExperiment();

            numberOfIterationsPerMap = 1500;
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
