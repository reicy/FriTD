using System;
using Manager.AI;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TD.Core;
using TD.Enums;

namespace Manager.GameStates
{
    class GameStateManager
    {
        private AICore core;
        private GameStateImage previousImage;

        public GameStateManager(AICore core)
        {
            this.core = core;
            this.previousImage = null;
        }

        public string ExecuteDecision(GameStateImage image)
        {
            previousImage = image;
            List<State> stavy = new List<State>();

            short x = 0; // stav
            int j = 2 * image.Towers.Length; // pocet nastavenych bitov
            for (int i = 0; i < image.Towers.Length; ++i)
            {
                switch (image.Towers[i])
                {
                    case -1: // ziadna
                        break;
                    case 0: // typ 1
                        x += 1;
                        break;
                    case 1: // typ 2
                        x += 2;
                        break;
                    case 2: // typ 3
                        x += 3;
                        break;
                }
                x <<= 2;
            }
            if (j < 12)
            {
                x <<= (12 - j);
            }

            // TODO bity 12, 13
            int k = image.Gold / image.TowerCost; // pocet vezi, ktore si mozem dovolit postavit
            if (k > 3) k = 3;
            x += (short)k;
            x <<= 1;

            // TODO bit 14
            k = image.Hp > image.NextWaveHpCost ? 1 : 0;
            x += (short)k;

            stavy.Add(new State(x));

            return "";
        }


        public string ExecuteDecision1(GameStateImage img)
        {
            previousImage = img;
            //Console.WriteLine("som v stave"+EncodeState(img).toString());
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



            var result = core.getNextState(EncodeState(img), relevantStates);
            return TransformStateToCommand(result);
        }

        public void ExecuteReward(GameStateImage image)
        {
            var prev = EncodeState(previousImage);
            var curr = EncodeAction(image);

            if (image.GameState == GameState.Won)
            {

                core.updateQ_values(prev, curr, 10000);
                return;
            }
            if (image.GameState == GameState.Lost)
            {
                core.updateQ_values(prev, curr, -10000);
                return;
            }

            
            double expectedHpCost = previousImage.NextWaveHpCost;
            double actualHpCost = previousImage.Hp - image.Hp;
            double reward = (actualHpCost / expectedHpCost)*200-100;
            //Console.WriteLine(reward);
            core.updateQ_values(prev, curr, reward);
        }

        private State EncodeState(GameStateImage img)
        {
            string str = "";
            foreach (var tower in img.Towers)
            {
                str += EncodeNumberTo2LengthStr(tower + 1);
                
            }
            str += EncodeNumberTo2LengthStr(img.Gold/img.TowerCost);
            if (img.Hp <= img.NextWaveHpCost)
            {
                str += "1";
            }
            else
            {
                str += "0";
            }
           // Debug.WriteLine(str);
            return new State(Convert.ToInt16(str,2));
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
            var str = state.toString();
            var towerPlace = "";
          //  Console.WriteLine("from "+EncodeState(previousImage).toString());
          //  Console.WriteLine("transform: "+str);

            for (int i = 0; i < 6; i++)
            {
                towerPlace = str.Substring(i*2, 2);
                if (towerPlace.Equals("00"))
                {
                    response += "s_" + i;
                }
                else
                {
                    var typId = Convert.ToInt16(towerPlace, 2);
                    typId--;
                    response += "b_"+i+"_"+typId;
                }
                


                response += " ";
            }
           // Console.WriteLine(response);

            return response.Trim();
        }
    }
}
