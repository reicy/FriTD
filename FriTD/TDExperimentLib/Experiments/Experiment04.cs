using System;
using System.Collections.Generic;
using Manager.MTCore.Core;
using Manager.Utils;

namespace TDExperimentLib.Experiments
{
    public class Experiment04 : ExperimentBase
    {
        MtManager manager = new MtManager();

        List<string> maps = new List<string>()
        {
            "Map",
            "Map1",
            "Map2",
            "Map3",
            "Map4",
            "Map5"
        };

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
            kohonenSaveFile = this.GetName() + "_k.txt";
            qLearningSaveFile = this.GetName() + "_q.txt";

            CustomLogger.logToFile = true;
            CustomLogger.filename = this.GetName() + "_stats1.txt";
            CustomLogger.ClearActualLogFile();

            numberOfIterationsPerMap = 200;
            numberOfIterationsPerMapWithKohonen = 200;
            kohonenRadius = 3;
            runExperiment();

            CustomLogger.filename = this.GetName() + "_stats2.txt";
            CustomLogger.ClearActualLogFile();

            kohonenLoadFile = kohonenSaveFile;
            qLearningLoadFile = qLearningSaveFile;

            numberOfIterationsPerMapWithKohonen = 0;
            kohonenLearningRate = 0.3;
            kohonenRadius = 2;
            runExperiment();
        }

        public void runExperiment()
        {
            manager.ExperimentRun(maps, numberOfIterationsPerMap, numberOfIterationsPerMapWithKohonen, useCosDist,
                qLearningRandomActionProbability, qLearningDiscountFactor, qLearningLearningRate,
                kohonenRows, kohonenCols, kohonenRadius, kohonenLearningRate, kohonenDistFactor, kohonenLoadFile,
                qLearningLoadFile, kohonenSaveFile, qLearningSaveFile);
        }
    }
}
