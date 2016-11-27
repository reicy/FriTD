using Manager.Kohonen;
using TD.Core;

namespace Manager.MTCore.KohonenUtils
{
    public class SimpleStateEncodern : IStateEncoder
    {
        public StateVector TranslateGameImage(GameStateImage image)
        {
            var vector = new StateVector();
            int counter = 0;

            foreach (var tower in image.Towers)
            {
                vector[counter] = tower + 1;
                counter++;
            }

            vector[counter] = image.Gold / image.TowerCost;
            counter++;

            if (image.Hp <= image.NextWaveHpCost)
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