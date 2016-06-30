using System;
using System.Collections.Generic;
using Manager.AIUtils;
using Manager.Core;
using Manager.GameStates;
using Manager.Kohonen;
using Manager.QLearning;
using TD.Core;
using TD.Enums;
using Action = Manager.AI.Action;

namespace Manager.MTCore
{
    public class MtAiAdapter
    {
        private readonly IntelligentActionInterpreter _intelligentActionInterpreter;
        private readonly GameStateProcessor gameStateProcessor;
        private readonly KohonenCore<StateVector> kohonen;
        private readonly QLearning<KohonenAiState> q_learning;
        private QAction _chosenAction;
        private double _rewardMultiplier;
        private bool learningEnabled;
        private GameStateImage previousImage;
        private KohonenAiState previousState;
        public KohonenUpdate KohonenUpdate { get; set; }

        public MtAiAdapter(QLearning<KohonenAiState> q_learning,
            KohonenCore<StateVector> kohonen)
        {
            this.q_learning = q_learning;
            previousState = null;
            this.kohonen = kohonen;
            gameStateProcessor = new GameStateProcessor();
            learningEnabled = true;
            _rewardMultiplier = 1;
            _intelligentActionInterpreter = new IntelligentActionInterpreter();
        }


        public string ExecuteDecision(GameStateImg gameStateImage)
        {
            var img = (GameStateImage) gameStateImage;
            //TODO zatial predava len max 1 - oprav na viac
            int[] dim;
            var state = gameStateProcessor.ProcessGameState(img);
            dim = kohonen.Winner(state);
            previousState = new KohonenAiState(dim);

            KohonenUpdate = new KohonenUpdate()
            {
                Col = dim[0], Row = dim[1], Vector = state
            };


            previousImage = img;
            //   Console.WriteLine("som v stave"+EncodeState(img).toString()+" goldy: "+img.Gold);


            var actions = new List<QAction>();
            var predpona = "2";
            var sellPart = "";
            var buyPart = "";

            //no action
            actions.Add(new Action(1000000));

            //matrix of possible combinations
            //type, coverage
            var availableCombinations = new int[3, 3];

            //tower places
            for (var i = 0; i < img.Ranges.GetLength(0); i++)
            {
                for (var j = 0; j < img.Ranges.GetLength(1); j++)
                {
                    if (img.Ranges[i, j] < IntelligentActionInterpreter.SmallRangePathCoverage)
                    {
                        availableCombinations[j, 0]++;
                    }
                    else
                    {
                        if (img.Ranges[i, j] < IntelligentActionInterpreter.MediumRangeCoverage)
                        {
                            availableCombinations[j, 1]++;
                        }
                        else
                        {
                            availableCombinations[j, 2]++;
                        }
                    }
                }
            }

            //nothing sold
            sellPart = "000";
            var maxTowersToBuild = img.Gold/img.TowerCost;
            //  Console.WriteLine(img.Gold +" "+ img.TowerCost+" "+maxTowersToBuild);
            for (var i = 0; i < 3; i++) 
            {
                for (var j = 0; j < 3; j++)
                {
                    for (var k = 0; k < 3; k++)
                    {
                        if (IsPossibleCombination(0, 0, 0, i, j, k, maxTowersToBuild, img, availableCombinations))
                            actions.Add(new Action(int.Parse(predpona + sellPart + i + j + k)));
                    }
                }
            }

            //TODO predaj

            /*       foreach(AI.Action action in actions)
                   {
                       Console.WriteLine(action.IntState);
                   }*/

            var result = q_learning.getNextAction(previousState, actions);
            return TransformActionToCommand((Action) result);
        }

        

        public void ExecuteReward(GameStateImg gameStateImage)
        {
            var image = (GameStateImage) gameStateImage;
            var prev = previousState;
            var curr = EncodeState(image);
            var action = _chosenAction;

            if (image.GameState == GameState.Won)
            {
                q_learning.updateQ_values(prev, action, curr, 1000);
                return;
            }
            if (image.GameState == GameState.Lost)
            {
                q_learning.updateQ_values(prev, action, curr, -500);
                return;
            }


            double expectedHpCost = previousImage.NextWaveHpCost;
            double actualHpCost = previousImage.Hp - image.Hp;
            var reward = (actualHpCost/expectedHpCost)*2 - 1;

            reward = reward*_rewardMultiplier;
            q_learning.updateQ_values(prev, action, curr, reward);
        }

        public void disableLearning()
        {
            learningEnabled = false;
        }

        //x type
        //y num
        //z covered path fields
        private bool IsPossibleCombination(int sx, int sy, int sz, int buyType, int buyNum, int buyCoverage,
            int maxTowersToBuild, GameStateImage img, int[,] availableCombinations)
        {
            //TODO ak sa predava


            if (buyNum == 0)
            {
                if (maxTowersToBuild == 0) return false;
                return /*availableCombinations[buyType, buyCoverage] >= 0 && */
                    availableCombinations[buyType, buyCoverage] >= IntelligentActionInterpreter.FewTowers;
            }
            if (buyNum == 1)
            {
                if (IntelligentActionInterpreter.FewTowers >= maxTowersToBuild) return false;
                return availableCombinations[buyType, buyCoverage] >= IntelligentActionInterpreter.MoreTowers;
            }
            if (IntelligentActionInterpreter.MoreTowers >= maxTowersToBuild) return false;
            return availableCombinations[buyType, buyCoverage] >= IntelligentActionInterpreter.MaxTowers;
        }


        private KohonenAiState EncodeState(GameStateImage img)
        {
            int[] dim;
            var state = gameStateProcessor.ProcessGameState(img);
            dim = kohonen.Winner(state);
            // kohonen.ReArrange(dim[0], dim[1], state);
            return new KohonenAiState(dim);
        }


        private string EncodeNumberTo2LengthStr(int num)
        {
            var str = Convert.ToString(num, 2);
            if (str.Length == 1) str = '0' + str;
            //TODO length > 2

            return str;
        }

        private string TransformActionToCommand(Action action)
        {
            _chosenAction = action;
            return _intelligentActionInterpreter.InterpretAction(previousImage, "" + (action.IntState));
        }

        public void SetRewardMultiplier(double d)
        {
            _rewardMultiplier = d;
        }
    }
}