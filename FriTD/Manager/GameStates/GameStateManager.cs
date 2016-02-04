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

        public void ExecuteReward(GameStateImage image)
        {
            var prev = EncodeState(previousImage);
            var curr = EncodeState(image);

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
                    response = "b_"+i+"_"+typId;
                }
                


                response += " ";
            }
            

            return response.Trim();
        }
    }
}
