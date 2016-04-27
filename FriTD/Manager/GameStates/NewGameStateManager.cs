using System;
using System.Collections.Generic;
using Manager.QLearning;
using TD.Core;
using System.Linq;
using System.Text;

namespace Manager.GameStates
{
    class NewGameStateManager
    {
        private QLearning<TDGameState> q_learning;
        private NewGameStateImage previousImage;

        public NewGameStateManager(QLearning<TDGameState> q_learning)
        {
            this.q_learning = q_learning ;
            this.previousImage = null;
        }

        /*   public string ExecuteDecision(GameStateImage image)
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
           */

        public string ExecuteDecision1(NewGameStateImage img)
        {
            previousImage = img;

            // otrimovany stav do q learningu a zaroven kohonena
            TDGameState gameState = EncodeState(img);
            double golds = img.Gold;

            //q learning vyberie najlepsiu akciu
            var nextAction = q_learning.getNextAction(gameState, TDActionFactory.getPossibleActions(golds).Cast<QAction>().ToList());


            return TransformStateToCommand(result);
        }

        public void ExecuteReward(GameStateImage image)
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
            double reward = (actualHpCost / expectedHpCost) * 2 - 1;
            //Console.WriteLine(reward);

            core.updateQ_values(prev, currS, curr, reward);
        }

        private TDGameState EncodeState(NewGameStateImage img)
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

        private string TransformStateToCommand(State state)
        {
            var response = "";
            var str = state.toString();
            var prevStr = EncodeState(previousImage).toString();
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
    }
}
}
