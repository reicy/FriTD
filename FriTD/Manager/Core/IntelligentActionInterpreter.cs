using System;
using TD.Core;

namespace Manager.Core
{
    class IntelligentActionInterpreter
    {
        public const int SMALL_RANGE_PATH_COVERAGE = 6;
        public const int MEDIUM_RANGE_COVERAGE = 12;

        public const int FEW_TOWERS = 1;
        public const int MORE_TOWERS = 3;
        public const int MAX_TOWERS = 6;

        private readonly Random _rnd = new Random();

        public string InterpretAction(GameStateImage image, string actionCode)
        {
            //TODO sell towers
            string response = "";
            if (actionCode == "1000000") return "";

            int towerTypeCategory = actionCode[4] - '0';
            int numberCategory = actionCode[5] - '0';
            int coverageCategory = actionCode[6] - '0';

            int maxTowersAllowedToBuild = image.Gold / image.TowerCost;
            int numberToBuild;
            if (numberCategory == 0)
            {
                numberToBuild = FEW_TOWERS;
            }
            else
            {
                if (numberCategory == 1)
                {
                    numberToBuild = (int)Math.Ceiling(1.0 * FEW_TOWERS + (MORE_TOWERS - FEW_TOWERS) * _rnd.NextDouble());
                }
                else
                {
                    numberToBuild = (int)Math.Ceiling(1.0 * MORE_TOWERS + (MAX_TOWERS - MORE_TOWERS) * _rnd.NextDouble());
                }
            }
            if (numberToBuild > maxTowersAllowedToBuild) numberToBuild = maxTowersAllowedToBuild;
            //TODO better logic
            //Console.WriteLine(@"{0} {1}", numberToBuild, _rnd.NextDouble());
            for (int i = 0; i < image.Towers.Length; i++)
            {

                if (image.Towers[i] >= 0) continue;
                if (numberToBuild == 0) break;

                if (RangeIsOk(image.Ranges[i, towerTypeCategory], coverageCategory))
                {
                    response += "b_" + i + "_" + towerTypeCategory;
                    numberToBuild--;
                    response += " ";
                }
            }
            //Console.WriteLine(@"{0} {1} ", actionCode, response.Trim());
            return response.Trim();
        }

        private bool RangeIsOk(int range, int bz)
        {
            if (bz == 0)
                return range <= SMALL_RANGE_PATH_COVERAGE;

            if (bz == 1)
                return range <= MEDIUM_RANGE_COVERAGE;

            return range > MEDIUM_RANGE_COVERAGE;
        }
    }
}
