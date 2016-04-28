﻿using System;
using Manager.AI;
using System.Collections.Generic;
using Manager.AIUtils;
using TD.Core;
using TD.Enums;

namespace Manager.GameStates
{
    class GameStateManager : IAiAdapter
    {
        private AICore core;
        private GameStateImage previousImage;

        public GameStateManager(AICore core)
        {
            this.core = core;
            this.previousImage = null;
        }

    

        public string ExecuteDecision1(GameStateImage img)
        {
            previousImage = img;
         //   Console.WriteLine("som v stave"+EncodeState(img).toString()+" goldy: "+img.Gold);
            List<State> relevantStates = new List<State>();
            
            
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
                    if (preImg.Gold > preImg.TowerCost*3) preImg.Gold = preImg.TowerCost*3;

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

            foreach (var state in relevantStates)
            {
               // if(state.toString().Length >0 ) Console.WriteLine(state.toString());
                //Console.WriteLine(state.toString());
            }


            if (relevantStates.Count > 1)
            {
                relevantStates.RemoveAt(0);
            }

            var result = core.getNextState(EncodeState(img), relevantStates);
            return TransformStateToCommand(result);
        }

        public void ExecuteReward1(GameStateImage image)
        {
            var prev = EncodeState(previousImage);
            var currS = EncodeState(image);
            var curr = EncodeAction(image);

            if (image.GameState == GameState.Won)
            {

                core.updateQ_values(prev, currS, curr, 1000);
                return;
            }
            if (image.GameState == GameState.Lost)
            {
                core.updateQ_values(prev, currS, curr, -500);
                return;
            }


            double expectedHpCost = previousImage.NextWaveHpCost;
            double actualHpCost = previousImage.Hp - image.Hp;
            double reward = (actualHpCost / expectedHpCost)*2-1;
            //Console.WriteLine(reward);

            core.updateQ_values(prev,currS, curr, reward);
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
            {
                str += "1";
            }
            else
            {
                str += "0";
            }
            // Debug.WriteLine(str);
            return new State(Convert.ToInt16(str, 2));
        }


        private State EncodeAction(GameStateImage img)
        {
            string str = "";
            foreach (var tower in img.Towers)
            {
                str += EncodeNumberTo2LengthStr(tower + 1);

            }
            str += "000";
            
            return new State(Convert.ToInt16(str, 2));
        }



        private string EncodeNumberTo2LengthStr(int num)
        {
            var str = Convert.ToString(num, 2);
            if (str.Length == 1) str = '0' + str;
            //TODO length > 2

            return str;
        }

        private string TransformStateToCommand(State state)
        {
            var response = "";
            var str = state.ToString();
            var prevStr = EncodeState(previousImage).ToString();
            var towerPlace = "";
            string lastTowerPlace = "";
          //  Console.WriteLine("from "+EncodeState(previousImage).toString());
         //   Console.WriteLine("transform: "+str);

            for (int i = 0; i < 6; i++)
            {
                towerPlace = str.Substring(i * 2, 2);
                lastTowerPlace = prevStr.Substring(i*2, 2);
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
            return ExecuteDecision1((GameStateImage) gameStateImage);
        }

        public void ExecuteReward(GameStateImg gameStateImage)
        {
            ExecuteReward1((GameStateImage)gameStateImage);
        }
    }
}
