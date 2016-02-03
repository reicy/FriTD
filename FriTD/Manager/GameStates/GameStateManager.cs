using Manager.AI;
using System.Collections;
using System.Collections.Generic;
using TD.Core;

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

        }

        private string TransformStateToCommand(State state)
        {
            return "";
        }
    }
}
