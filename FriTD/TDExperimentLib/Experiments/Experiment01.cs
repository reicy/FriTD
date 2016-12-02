using System.Collections.Generic;
using Manager.Core;

namespace TDExperimentLib.Experiments
{
    public class Experiment01 : ExperimentBase
    {
        public int Iterations { get; private set; }
        public int GamesPerIteration { get; private set; }
        public bool ChangeLearningRate { get; private set; }
        public int DecreaseLearningRateAfter { get; private set; }
        public double DecreaseLearningRatePercentage { get; private set; }
        public string FirstMap { get; private set; }
        public string SecondMap { get; private set; }
        public double InitialLearningRate { get; private set; }
        public double InitialRandomActionProbability { get; private set; }
        public string KohonenOutputFile1 { get; private set; }
        public string KohonenOutputFile2 { get; private set; }
        public string QLearningOutputFile1 { get; private set; }
        public string QLearningOutputFile2 { get; private set; }

        public Experiment01() : base("Train on one map and test on another")
        {
            Iterations = 400;
            GamesPerIteration = 100;
            ChangeLearningRate = false;
            DecreaseLearningRateAfter = 0;
            DecreaseLearningRatePercentage = 0.1;
            FirstMap = "Map1";
            SecondMap = "Map2";
            InitialLearningRate = 0.5;
            InitialRandomActionProbability = 0.3;
            KohonenOutputFile1 = "";
            KohonenOutputFile2 = "";
            QLearningOutputFile1 = "";
            QLearningOutputFile2 = "";
        }

        public override KeyValuePair<bool, string> SetData(Dictionary<string, object> propVals)
        {
            int tmpIterations;
            int tmpGamesPerIteration;
            bool tmpChangeLearningRate;
            int tmpDecreaseLearningRateAfter;
            double tmpDecreaseLearningRatePercentage;
            string tmpFirstMap;
            string tmpSecondMap;
            double tmpInitialLearningRate;
            double tmpInitialRandomActionProbability;
            string tmpKohonenOutputFile1;
            string tmpKohonenOutputFile2;
            string tmpQLearningOutputFile1;
            string tmpQLearningOutputFile2;

            var ok = int.TryParse(propVals["Iterations"].ToString(), out tmpIterations);
            if (!ok) return new KeyValuePair<bool, string>(false, "The 'Iterations' property has invalid value!");
            ok = int.TryParse(propVals["GamesPerIteration"].ToString(), out tmpGamesPerIteration);
            if (!ok) return new KeyValuePair<bool, string>(false, "The 'GamesPerIteration' property has invalid value!");
            ok = bool.TryParse(propVals["ChangeLearningRate"].ToString(), out tmpChangeLearningRate);
            if (!ok) return new KeyValuePair<bool, string>(false, "The 'ChangeLearningRate' property has invalid value!");
            ok = int.TryParse(propVals["DecreaseLearningRateAfter"].ToString(), out tmpDecreaseLearningRateAfter);
            if (!ok) return new KeyValuePair<bool, string>(false, "The 'DecreaseLearningRateAfter' property has invalid value!");
            ok = double.TryParse(propVals["DecreaseLearningRatePercentage"].ToString(), out tmpDecreaseLearningRatePercentage);
            if (!ok) return new KeyValuePair<bool, string>(false, "The 'DecreaseLearningRatePercentage' property has invalid value!");
            ok = double.TryParse(propVals["InitialLearningRate"].ToString(), out tmpInitialLearningRate);
            if (!ok) return new KeyValuePair<bool, string>(false, "The 'InitialLearningRate' property has invalid value!");
            ok = double.TryParse(propVals["InitialRandomActionProbability"].ToString(), out tmpInitialRandomActionProbability);
            if (!ok) return new KeyValuePair<bool, string>(false, "The 'InitialRandomActionProbability' property has invalid value!");
            tmpFirstMap = propVals["FirstMap"].ToString();
            tmpSecondMap = propVals["SecondMap"].ToString();
            tmpKohonenOutputFile1 = propVals["KohonenOutputFile1"].ToString();
            tmpKohonenOutputFile2 = propVals["KohonenOutputFile2"].ToString();
            tmpQLearningOutputFile1 = propVals["QLearningOutputFile1"].ToString();
            tmpQLearningOutputFile2 = propVals["QLearningOutputFile2"].ToString();

            Iterations = tmpIterations;
            GamesPerIteration = tmpGamesPerIteration;
            ChangeLearningRate = tmpChangeLearningRate;
            DecreaseLearningRateAfter = tmpDecreaseLearningRateAfter;
            DecreaseLearningRatePercentage = tmpDecreaseLearningRatePercentage;
            FirstMap = tmpFirstMap;
            SecondMap = tmpSecondMap;
            InitialLearningRate = tmpInitialLearningRate;
            InitialRandomActionProbability = tmpInitialRandomActionProbability;
            KohonenOutputFile1 = tmpKohonenOutputFile1;
            KohonenOutputFile2 = tmpKohonenOutputFile2;
            QLearningOutputFile1 = tmpQLearningOutputFile1;
            QLearningOutputFile2 = tmpQLearningOutputFile2;
            return new KeyValuePair<bool, string>(true, "");
        }

        public override void Run()
        {
            var mng = ManagerBuilder.BuildAiLearningManager();
            mng.AiLearningRunTwoMaps(
                Iterations, GamesPerIteration, FirstMap, SecondMap, ChangeLearningRate, DecreaseLearningRateAfter, DecreaseLearningRatePercentage,
                InitialLearningRate, InitialRandomActionProbability, KohonenOutputFile1, KohonenOutputFile2, QLearningOutputFile1, QLearningOutputFile2
            );
        }
    }
}
