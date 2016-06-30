using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using TD.Core;

namespace Manager.Core
{
    class IntelligentActionInterpreter
    {

        public const int SmallRangePathCoverage = 6;
        public const int MediumRangeCoverage = 12;

        public const int FewTowers = 1;
        public const int MoreTowers = 3;
        public const int MaxTowers = 6;

        private Random _rnd = new Random();


        public string InterpretAction(GameStateImage image, string actionCode)
        {
            //TODO sell towers
            string response = ""; 
            if (actionCode == "1000000") return "";

            int towerTypeCategory = actionCode[4] - '0';
            int numberCategory = actionCode[5] - '0';
            int coverageCategory = actionCode[6] - '0';

            int maxTowersAllowedToBuild = image.Gold/image.TowerCost;
            int numberToBuild = 0;
            if (numberCategory == 0)
            {
                numberToBuild = FewTowers;
            }
            else
            {
                if (numberCategory == 1)
                {
                    numberToBuild = (int)Math.Ceiling(1.0 * FewTowers + (MoreTowers-FewTowers) * _rnd.NextDouble());
                }
                else
                {
                    numberToBuild = (int) Math.Ceiling(1.0*MoreTowers + (MaxTowers-MoreTowers) * _rnd.NextDouble());
                }
            }
            if (numberToBuild > maxTowersAllowedToBuild) numberToBuild = maxTowersAllowedToBuild;
            //TODO better logic
            //Console.WriteLine(numberToBuild+" "+_rnd.NextDouble());
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
           // Console.WriteLine(actionCode+" "+response.Trim()+" ");
            return response.Trim();
        }

        private bool RangeIsOk(int range, int bz)
        {
            if (bz == 0)
            {
                return (range <= SmallRangePathCoverage);
            }
            else
            {
                if (bz == 1)
                {
                    return (range <= MediumRangeCoverage);
                }
                else
                {
                    return (range > MediumRangeCoverage);
                }
            }
        }
    }
}
