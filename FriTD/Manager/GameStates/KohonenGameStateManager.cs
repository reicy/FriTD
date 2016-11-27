using System;
using System.Collections.Generic;
using Manager.AIUtils;
using Manager.Kohonen;
using TD.Core;
using TD.Enums;
using Manager.QLearning;

namespace Manager.GameStates
{
    public class KohonenGameStateManager : IAiAdapter
    {
        private readonly QLearning<KohonenAiState> _qLearning;
        private readonly KohonenCore<StateVector> _kohonen;
        private readonly GameStateProcessor _gameStateProcessor;
        private KohonenAiState _previousState;
        private GameStateImage _previousImage;
        private bool _learningEnabled;
        private double _rewardMultiplier;

        public KohonenGameStateManager(QLearning<KohonenAiState> qLearning, KohonenCore<StateVector> kohonen)
        {
            _qLearning = qLearning;
            _previousState = null;
            _kohonen = kohonen;
            _gameStateProcessor = new GameStateProcessor();
            _learningEnabled = true;
            _rewardMultiplier = 1;
        }

        public void DisableLearning()
        {
            _learningEnabled = false;
        }

        public string ExecuteDecision(GameStateImg gameStateImage)
        {
            GameStateImage img = (GameStateImage)gameStateImage;

            var state = _gameStateProcessor.ProcessGameState(img);
            var dim = _kohonen.Winner(state);
            _previousState = new KohonenAiState(dim);
            if (_learningEnabled) _kohonen.ReArrange(dim[0], dim[1], state);

            _previousImage = img;
            //Console.WriteLine(@"som v stave {0} goldy: {1}", EncodeState(img), img.Gold);
            var relevantStates = new List<AI.Action>();

            var tempImg = img.CloneThis();
            GameStateImage seccImg;

            //no action
            relevantStates.Add(EncodeAction(tempImg));

            // no tower sold

            //build 1.
            for (int i = 0; i < 6; i++)
            {
                tempImg = img.CloneThis();
                if (tempImg.Towers[i] == -1 && tempImg.TowerCost <= tempImg.Gold)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        tempImg = img.CloneThis();
                        tempImg.Towers[i] = j;
                        tempImg.Gold -= tempImg.TowerCost;
                        relevantStates.Add(EncodeAction(tempImg));
                        //build 2.
                        for (int k = 0; k < 6; k++)
                        {
                            if (tempImg.Towers[i] == -1 && tempImg.TowerCost <= tempImg.Gold)
                            {
                                for (int l = 0; l < 3; l++)
                                {
                                    seccImg = tempImg.CloneThis();
                                    seccImg.Towers[k] = l;
                                    seccImg.Gold -= seccImg.TowerCost;
                                    relevantStates.Add(EncodeAction(seccImg));
                                }
                            }
                        }
                    }
                }
            }

            // 1 tower sold

            for (int t = 0; t < 6; t++)
            {
                if (img.Towers[t] >= 0)
                {
                    var preImg = img.CloneThis();
                    preImg.Towers[t] = -1;
                    preImg.Gold += preImg.TowerRefundCost;
                    if (preImg.Gold > preImg.TowerCost * 3) preImg.Gold = preImg.TowerCost * 3;

                    for (int i = 0; i < 6; i++)
                    {
                        tempImg = preImg.CloneThis();
                        if (tempImg.Towers[i] == -1 && tempImg.TowerCost <= tempImg.Gold)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                tempImg = preImg.CloneThis();
                                tempImg.Towers[i] = j;
                                tempImg.Gold -= tempImg.TowerCost;
                                relevantStates.Add(EncodeAction(tempImg));
                                //build 2.
                                for (int k = 0; k < 6; k++)
                                {
                                    if (tempImg.Towers[i] == -1 && tempImg.TowerCost <= tempImg.Gold)
                                    {
                                        for (int l = 0; l < 3; l++)
                                        {
                                            seccImg = tempImg.CloneThis();
                                            seccImg.Towers[k] = l;
                                            seccImg.Gold -= seccImg.TowerCost;
                                            relevantStates.Add(EncodeAction(seccImg));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (relevantStates.Count > 1)
            {
                relevantStates.RemoveAt(0);
            }

            var actions = new List<QAction>();
            foreach (var item in relevantStates)
            {
                actions.Add(item);
            }

            var result = _qLearning.GetNextAction(_previousState, actions);
            return TransformActionToCommand((AI.Action)result);
        }

        private KohonenAiState EncodeState(GameStateImage img)
        {
            var state = _gameStateProcessor.ProcessGameState(img);
            var dim = _kohonen.Winner(state);
            //kohonen.ReArrange(dim[0], dim[1], state);
            return new KohonenAiState(dim);
        }

        private AI.Action EncodeAction(GameStateImage img)
        {
            string str = "";
            foreach (var tower in img.Towers)
            {
                str += EncodeNumberTo2LengthStr(tower + 1);
            }
            str += "000";

            return new AI.Action(Convert.ToInt16(str, 2));
        }

        private string EncodeNumberTo2LengthStr(int num)
        {
            var str = Convert.ToString(num, 2);
            if (str.Length == 1) str = '0' + str;
            //TODO length > 2

            return str;
        }

        private string TransformActionToCommand(AI.Action state)
        {
            var response = "";
            var str = state.ToString();
            //TODO
            var prevStr = EncodeAction(_previousImage).ToString();
            string towerPlace;
            //Console.WriteLine(@"from {0}", EncodeState(_previousImage));
            //Console.WriteLine(@"transform: {0}", str);

            for (int i = 0; i < 6; i++)
            {
                towerPlace = str.Substring(i * 2, 2);
                var lastTowerPlace = prevStr.Substring(i * 2, 2);
                if (towerPlace != lastTowerPlace)
                {
                    response += "s_" + i;
                }
                response += " ";
            }

            for (int i = 0; i < 6; i++)
            {
                towerPlace = str.Substring(i * 2, 2);
                if (towerPlace.Equals("00"))
                {

                }
                else
                {
                    var typId = Convert.ToInt16(towerPlace, 2);
                    typId--;
                    response += "b_" + i + "_" + typId;
                    //response = "b_" + i + "_" + typId;
                }
                response += " ";
            }

            //Console.WriteLine(response);

            return response.Trim();
        }

        public void ExecuteReward(GameStateImg gameStateImage)
        {
            GameStateImage image = (GameStateImage)gameStateImage;
            var prev = _previousState;
            var curr = EncodeState(image);
            var action = EncodeAction(image);

            if (image.GameState == GameState.Won)
            {
                _qLearning.updateQ_values(prev, action, curr, 1000);
                return;
            }
            if (image.GameState == GameState.Lost)
            {
                _qLearning.updateQ_values(prev, action, curr, -500);
                return;
            }

            double expectedHpCost = _previousImage.NextWaveHpCost;
            double actualHpCost = _previousImage.Hp - image.Hp;
            double reward = (actualHpCost / expectedHpCost) * 2 - 1;

            reward = reward * _rewardMultiplier;
            _qLearning.updateQ_values(prev, action, curr, reward);
        }

        public void SetRewardMultiplier(double d)
        {
            _rewardMultiplier = d;
        }
    }
}