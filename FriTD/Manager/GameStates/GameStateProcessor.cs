using Manager.Kohonen;
using TD.Core;

namespace Manager.GameStates
{
    public class GameStateProcessor
    {
        public StateVector ProcessGameState(GameStateImage img)
        {
            StateVector vector = new StateVector();
            int counter = 0;
            foreach (var tower in img.Towers)
            {
                vector[counter] = tower + 1;
                counter++;

            }
            vector[counter] = (img.Gold/img.TowerCost);
            counter++;
            if (img.Hp <= img.NextWaveHpCost)
            {
                vector[counter] = 1;
            }
            else
            {
                vector[counter] = 0;
            }
            return vector;
        }


    } 
 }
