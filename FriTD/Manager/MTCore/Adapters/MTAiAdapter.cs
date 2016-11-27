using System.Collections.Generic;
using Manager.Core;
using Manager.Kohonen;
using Manager.MTCore.Core;
using Manager.MTCore.KohonenUtils;
using Manager.QLearning;
using TD.Core;
using TD.Enums;
using Action = Manager.AI.Action;

namespace Manager.MTCore.Adapters
{
    public class MtAiAdapter
    {
        private readonly IntelligentActionInterpreter _intelligentActionInterpreter;
        private readonly IStateEncoder _stateEncoder;
        private readonly KohonenCore<StateVector> _kohonen;
        private readonly QLearning<KohonenAiState> _qLearning;
        private QAction _chosenAction;
        private double _rewardMultiplier;
        private bool _learningEnabled;
        private GameStateImage _previousImage;
        private KohonenAiState _previousState;
        private readonly bool _heuristicActive;
        private readonly bool _cosinusDistActive;

        public KohonenUpdate KohonenUpdate { get; set; }

        public MtAiAdapter(QLearning<KohonenAiState> qLearning,
            KohonenCore<StateVector> kohonen, IStateEncoder stateEncoder, bool heuristicActive, bool cosinusDistActive)
        {
            _qLearning = qLearning;
            _previousState = null;
            _kohonen = kohonen;
            _stateEncoder = stateEncoder;
            _learningEnabled = true;
            _rewardMultiplier = 1;
            _intelligentActionInterpreter = new IntelligentActionInterpreter();
            _heuristicActive = heuristicActive;
            _cosinusDistActive = cosinusDistActive;
        }

        public string ExecuteDecision(GameStateImage img)
        {
            int[] dim;
            var state = _stateEncoder.TranslateGameImage(img);

            if (_heuristicActive)
            {
                if (_cosinusDistActive)
                {
                    dim = _kohonen.WinnerHeuristic(state, _kohonen.DistCosine);
                }
                else
                {
                    dim = _kohonen.WinnerHeuristic(state, _kohonen.DistEuclidean);
                }
            }
            else
            {
                if (_cosinusDistActive)
                {
                    dim = _kohonen.Winner(state, _kohonen.DistCosine, false);
                }
                else
                {
                    dim = _kohonen.Winner(state, _kohonen.DistEuclidean, true);
                }
            }

            _previousState = new KohonenAiState(dim);

            KohonenUpdate = new KohonenUpdate
            {
                Col = dim[0],
                Row = dim[1],
                Vector = state
            };

            _previousImage = img;
            //Console.WriteLine(@"som v stave {0} goldy: {1}", EncodeState(img), img.Gold);

            var actions = new List<QAction>();
            var predpona = "2";

            //no action
            actions.Add(new Action(1000000));

            //matrix of possible combinations
            //type, coverage
            var availableCombinations = new int[3, 3];

            //tower places
            for (var i = 0; i < img.Ranges.GetLength(0); i++)
            {
                //check this, should skip not empty tower places
                if (img.Towers[i] != -1) continue;

                for (var j = 0; j < img.Ranges.GetLength(1); j++)
                {
                    if (img.Ranges[i, j] < IntelligentActionInterpreter.SMALL_RANGE_PATH_COVERAGE)
                    {
                        availableCombinations[j, 0]++;
                    }
                    else
                    {
                        if (img.Ranges[i, j] < IntelligentActionInterpreter.MEDIUM_RANGE_COVERAGE)
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
            var sellPart = "000";
            var maxTowersToBuild = img.Gold / img.TowerCost;
            //Console.WriteLine(@"{0} {1} {2}", img.Gold, img.TowerCost, maxTowersToBuild);
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

            /*foreach (var action in actions)
            {
                Console.WriteLine(action.IntState);
            }*/

            var result = _qLearning.GetNextAction(_previousState, actions);
            return TransformActionToCommand((Action)result);
        }

        public void ExecuteReward(GameStateImage image)
        {
            var prev = _previousState;
            var curr = EncodeState(image);
            var action = _chosenAction;

            if (image.GameState == GameState.Won)
            {
                _qLearning.updateQ_values(prev, action, curr, 1000);
                return;
            }
            if (image.GameState == GameState.Lost)
            {
                _qLearning.updateQ_values(prev, action, curr, -1000);
                return;
            }

            double expectedHpCost = _previousImage.NextWaveHpCost;
            double actualHpCost = _previousImage.Hp - image.Hp;
            var reward = (actualHpCost / expectedHpCost) * 2 - 1;

            reward = reward * _rewardMultiplier;
            _qLearning.updateQ_values(prev, action, curr, reward);
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
                    availableCombinations[buyType, buyCoverage] >= IntelligentActionInterpreter.FEW_TOWERS;
            }
            if (buyNum == 1)
            {
                if (IntelligentActionInterpreter.FEW_TOWERS >= maxTowersToBuild) return false;
                return availableCombinations[buyType, buyCoverage] >= IntelligentActionInterpreter.MORE_TOWERS;
            }
            if (IntelligentActionInterpreter.MORE_TOWERS >= maxTowersToBuild) return false;
            return availableCombinations[buyType, buyCoverage] >= IntelligentActionInterpreter.MAX_TOWERS;
        }

        private KohonenAiState EncodeState(GameStateImage img)
        {
            int[] dim;
            var state = _stateEncoder.TranslateGameImage(img);

            if (_heuristicActive)
            {
                if (_cosinusDistActive)
                {
                    dim = _kohonen.WinnerHeuristic(state, _kohonen.DistCosine);
                }
                else
                {
                    dim = _kohonen.WinnerHeuristic(state, _kohonen.DistEuclidean);
                }
            }
            else
            {
                if (_cosinusDistActive)
                {
                    dim = _kohonen.Winner(state, _kohonen.DistCosine, false);
                }
                else
                {
                    dim = _kohonen.Winner(state, _kohonen.DistEuclidean, true);
                }
            }
            //dim = _kohonen.Winner(state);
            return new KohonenAiState(dim);
        }

        private string TransformActionToCommand(Action action)
        {
            _chosenAction = action;
            return _intelligentActionInterpreter.InterpretAction(_previousImage, "" + action.IntState);
        }

        public void SetRewardMultiplier(double d)
        {
            _rewardMultiplier = d;
        }
    }
}
