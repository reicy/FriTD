﻿using System;
using System.Collections.Generic;
using Manager.QLearning;
using TD.Core;
using Manager.AI;
using Manager.AIUtils;
using TD.Enums;

namespace Manager.GameStates
{
    class NewGameStateManager : IAiAdapter
    {
        private readonly QLearning<State, AI.Action> _qLearning;
        private GameStateImage _previousImage;

        public NewGameStateManager(QLearning<State, AI.Action> qLearning)
        {
            _qLearning = qLearning;
            _previousImage = null;
        }

        /*public string ExecuteDecision(GameStateImage image)
        {
            _previousImage = image;

            var states = new List<State>();

            var state = EncodeState(image);

            if (image.GameState != GameState.Lost && image.GameState != GameState.Won)
            {
                // stavy - mozne operacie - postavenie max. 2 vezi, zburanie max. 1 veze
                states.Add(state);

                int numTowers = image.Towers.Length;

                var nepostavene = new List<int>();
                var postavene = new List<int>();
                for (int i = 0; i < numTowers; ++i)
                {
                    if (image.Towers[i] == -1)
                        nepostavene.Add(i);
                    else
                        postavene.Add(i);
                }

                // iba buranie
                GameStateImage tmpImage = image.CloneThis();
                tmpImage.Gold += tmpImage.TowerRefundCost;
                for (int z = 0; z < postavene.Count; ++z)
                {
                    var pom = tmpImage.Towers[postavene[z]];
                    tmpImage.Towers[postavene[z]] = -1;
                    states.Add(EncodeState(tmpImage));
                    tmpImage.Towers[postavene[z]] = pom;
                }

                if (nepostavene.Count > 0) // ak je aspon jedna nepostavena, mozeme aj stavat, inak len burat
                {
                    if (postavene.Count == 0) // ak nie je ziadna postavena, mozeme len stavat
                    {
                        for (int p1 = 0; p1 < nepostavene.Count; ++p1)
                        {
                            for (int p2 = p1; p2 < nepostavene.Count; ++p2)
                            {
                                states.AddRange(StatesHelper(nepostavene, postavene, -1, p1, p2));
                            }
                        }
                    }
                    else // mozeme stavat aj burat
                    {
                        for (int z = 0; z < postavene.Count; ++z) // zburat
                        {
                            for (int p1 = 0; p1 < nepostavene.Count; ++p1)
                            {
                                for (int p2 = p1; p2 < nepostavene.Count; ++p2)
                                {
                                    states.AddRange(StatesHelper(nepostavene, postavene, z, p1, p2));
                                }
                            }
                        }
                    }
                }
            }

            State s = core.getNextState(state, states);

            return TransformStateToCommand(s);
        }

        // p - postavene veze, n - nepostavene veze
        private List<State> StatesHelper(List<int> n, List<int> p, int z, int p1, int p2)
        {
            var states = new List<State>();

            int steps = z == -1 ? 1 : 2;

            for (int k = 0; k < steps; ++k)
            {
                GameStateImage tmpImage = _previousImage.CloneThis();

                // zburanie
                if (k == 1)
                {
                    tmpImage.Towers[p[z]] = -1;
                    tmpImage.Gold += tmpImage.TowerRefundCost;
                }

                var canAffordTowers = tmpImage.Gold / tmpImage.TowerCost;

                // postavenie prvej
                if (canAffordTowers > 0)
                {
                    tmpImage.Gold -= tmpImage.TowerCost;
                    for (int i = 0; i < 3; ++i)
                    {
                        tmpImage.Towers[n[p1]] = i;
                        states.Add(EncodeState(tmpImage));
                    }

                    // postavenie druhej
                    if (p1 != p2 && canAffordTowers > 1)
                    {
                        tmpImage.Gold -= tmpImage.TowerCost;
                        for (int i = 0; i < 3; ++i)
                        {
                            tmpImage.Towers[n[p1]] = i;

                            for (int j = 0; j < 3; ++j)
                            {
                                tmpImage.Towers[n[p2]] = j;
                                states.Add(EncodeState(tmpImage));
                            }
                        }
                    }
                }
            }

            return states;
        }*/

        /*public string ExecuteDecision1(GameStateImage img)
        {
            _previousImage = img;

            // otrimovany stav do q learningu a zaroven kohonena
            var gameState = EncodeState(img);
            double golds = img.Gold;

            //q learning vyberie najlepsiu akciu
            var nextAction = _qLearning.getNextAction(gameState, TDActionFactory.getPossibleActions(golds).Cast<QAction>().ToList());
            
            return TransformStateToCommand(result);
        }*/

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

        public void ExecuteReward1(GameStateImage image)
        {
            var prev = EncodeState(_previousImage);
            var currS = EncodeState(image);
            var action = EncodeAction(image);

            if (image.GameState == GameState.Won)
            {
                _qLearning.updateQ_values(prev, action, currS, 1000);
                return;
            }
            if (image.GameState == GameState.Lost)
            {
                _qLearning.updateQ_values(prev, action, currS, -500);
                return;
            }

            double expectedHpCost = _previousImage.NextWaveHpCost;
            double actualHpCost = _previousImage.Hp - image.Hp;
            double reward = (actualHpCost / expectedHpCost) * 2 - 1;
            //Console.WriteLine(reward);

            _qLearning.updateQ_values(prev, action, currS, reward);
        }

        private State EncodeState(GameStateImage img)
        {
            string str = "";
            foreach (var tower in img.Towers)
            {
                str += EncodeNumberTo2LengthStr(tower + 1);
            }

            str += EncodeNumberTo2LengthStr(img.Gold / img.TowerCost);
            if (img.Hp <= img.NextWaveHpCost)
                str += "1";
            else
                str += "0";

            // Debug.WriteLine(str);
            return new State(Convert.ToInt16(str, 2));
        }

        /*private State EncodeAction(GameStateImage img)
        {
            string str = "";
            foreach (var tower in img.Towers)
            {
                str += EncodeNumberTo2LengthStr(tower + 1);
            }
            str += "000";

            return new State(Convert.ToInt16(str, 2));
        }*/

        private string EncodeNumberTo2LengthStr(int num)
        {
            var str = Convert.ToString(num, 2);
            if (str.Length == 1) str = '0' + str;
            //TODO length > 2

            return str;
        }

        private string TransformStateToCommand(AI.Action state)
        {
            var response = "";
            var str = state.ToString();
            var prevStr = EncodeState(_previousImage).ToString();
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

        public string ExecuteDecision(GameStateImg gameStateImage)
        {
            return ExecuteDecision1((GameStateImage)gameStateImage);
        }

        public void ExecuteReward(GameStateImg gameStateImage)
        {
            ExecuteReward1((GameStateImage)gameStateImage);
        }

        public string ExecuteDecision1(GameStateImage img)
        {
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

            /*foreach (var state in relevantStates)
            {
                if (state.ToString().Length > 0) Console.WriteLine(state);
                Console.WriteLine(state);
            }*/

            if (relevantStates.Count > 1)
            {
                relevantStates.RemoveAt(0);
            }

            var actions = new List<AI.Action>();
            foreach (var item in relevantStates)
            {
                actions.Add(item);
            }

            var result = _qLearning.GetNextAction(EncodeState(img), actions);
            return TransformStateToCommand(result);
        }
    }
}
