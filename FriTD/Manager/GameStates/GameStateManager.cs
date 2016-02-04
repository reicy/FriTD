using System;
using Manager.AI;
using System.Collections.Generic;
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

            List<State> states = new List<State>();

            State state = EncodeState(image);

            if (image.GameState != TD.Enums.GameState.Lost && image.GameState != TD.Enums.GameState.Won)
            {
                // stavy - mozne operacie - postavenie max. 2 vezi, zburanie max. 1 veze
                states.Add(state);

                int numTowers = image.Towers.Length;

                List<int> nepostavene = new List<int>();
                List<int> postavene = new List<int>();
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
                int pom;
                for (int z = 0; z < postavene.Count; ++z)
                {
                    pom = tmpImage.Towers[postavene[z]];
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
            List<State> states = new List<State>();

            GameStateImage tmpImage;

            int canAffordTowers;

            int steps = z == -1 ? 1 : 2;

            for (int k = 0; k < steps; ++k)
            {

                tmpImage = previousImage.CloneThis();

                // zburanie
                if (k == 1)
                {
                    tmpImage.Towers[p[z]] = -1;
                    tmpImage.Gold += tmpImage.TowerRefundCost;
                }

                canAffordTowers = tmpImage.Gold / tmpImage.TowerCost;

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
            var str = state.toString();
            var towerPlace = "";
          //  Console.WriteLine("from "+EncodeState(previousImage).toString());
          //  Console.WriteLine("transform: "+str);

            for (int i = 0; i < 6; i++)
            {
                towerPlace = str.Substring(i * 2, 2);
                if (towerPlace.Equals("00"))
                {
                    response += "s_" + i;
                }
                else
                {
                    var typId = Convert.ToInt16(towerPlace, 2);
                    typId--;
                    response += "b_"+i+"_"+typId;
                    response = "b_" + i + "_" + typId;
                }



                response += " ";
            }
           // Console.WriteLine(response);

            return response.Trim();
        }
    }
}
