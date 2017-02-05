using System.Collections.Generic;
using Manager.MTCore.Core;

namespace TDExperimentLib.Experiments
{
    public class Experiment03 : ExperimentBase
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

        public Experiment03() : base("All maps at once")
        {
        }

        public override void Run()
        {
            kohonenSaveFile = "koh.txt";
            qLearningSaveFile = "q.txt";


            numberOfIterationsPerMap = 2500;
            numberOfIterationsPerMapWithKohonen = 2500;
            kohonenRadius = 3;
            runExperiment();

            kohonenLoadFile = kohonenSaveFile;
            qLearningLoadFile = qLearningSaveFile;

            numberOfIterationsPerMap = 2500;
            numberOfIterationsPerMapWithKohonen = 2500;
            kohonenLearningRate = 0.3;
            kohonenRadius = 2;
            runExperiment();

            numberOfIterationsPerMap = 2500;
            numberOfIterationsPerMapWithKohonen = 0;
            qLearningRandomActionProbability = 0.2;
            qLearningLearningRate = 0.3;
            runExperiment();

            numberOfIterationsPerMap = 2500;
            numberOfIterationsPerMapWithKohonen = 0;
            qLearningRandomActionProbability = 0.1;
            qLearningLearningRate = 0.3;
            runExperiment();

            numberOfIterationsPerMap = 2500;
            numberOfIterationsPerMapWithKohonen = 0;
            qLearningRandomActionProbability = 0.05;
            qLearningLearningRate = 0.2;
            runExperiment();

            numberOfIterationsPerMap = 1000;
            numberOfIterationsPerMapWithKohonen = 0;
            qLearningRandomActionProbability = 0.01;
            qLearningLearningRate = 0.01;
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
