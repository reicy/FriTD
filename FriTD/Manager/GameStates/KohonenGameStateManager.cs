using System;
using System.Collections.Generic;
using Manager.AI;
using Manager.AIUtils;
using Manager.Kohonen;
using TD.Core;
using TD.Enums;
using Manager.QLearning;

namespace Manager.GameStates
{
    public class KohonenGameStateManager:IAiAdapter
    {
        private QLearning<KohonenAiState> q_learning;
        private KohonenAiState previousState;
        private KohonenCore<StateVector> kohonen;
        private GameStateProcessor gameStateProcessor;
        private GameStateImage previousImage;

        public KohonenGameStateManager(QLearning<KohonenAiState> q_learning, KohonenCore<StateVector> kohonen)
        {
            this.q_learning = q_learning;
            this.previousState = null;
            this.kohonen = kohonen;
            gameStateProcessor = new GameStateProcessor();
            
        }


        public string ExecuteDecision(GameStateImg gameStateImage)
        {
            GameStateImage img = (GameStateImage) gameStateImage;
            previousState = EncodeState(img);
            previousImage = img;
            //   Console.WriteLine("som v stave"+EncodeState(img).toString()+" goldy: "+img.Gold);
            List<AI.Action> relevantStates = new List<AI.Action>();


            var tempImg = img.CloneThis();
            var seccImg = img.CloneThis();
            var preImg = img.CloneThis();

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
                    preImg = img.CloneThis();
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
            List<QAction> actions = new List<QAction>();
            foreach (var item in relevantStates)
            {
                actions.Add(item);
            }
            var result = q_learning.getNextAction(previousState, actions);
            return TransformActionToCommand((AI.Action)result);
        }

        private KohonenAiState EncodeState(GameStateImage img)
        {
            int[] dim;
            var state = gameStateProcessor.ProcessGameState(img);
            dim = kohonen.Winner(state);
            kohonen.ReArrange(dim[0], dim[1], state);
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
            var prevStr = EncodeAction(previousImage).ToString();
            var towerPlace = "";
            string lastTowerPlace = "";
            //  Console.WriteLine("from "+EncodeState(previousImage).toString());
            //   Console.WriteLine("transform: "+str);

            for (int i = 0; i < 6; i++)
            {
                towerPlace = str.Substring(i * 2, 2);
                lastTowerPlace = prevStr.Substring(i * 2, 2);
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
            GameStateImage image = (GameStateImage) gameStateImage;
            var prev = previousState;
            var curr = EncodeState(image);
            var action = EncodeAction(image);

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
            double reward = (actualHpCost / expectedHpCost) * 2 - 1;
            
            q_learning.updateQ_values(prev, action, curr, reward);
        }
    }
}