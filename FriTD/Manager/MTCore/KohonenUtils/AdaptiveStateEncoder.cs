using System;
using Manager.Core;
using Manager.Kohonen;
using TD.Core;

namespace Manager.MTCore.KohonenUtils
{
    public class AdaptiveStateEncoder : IStateEncoder
    {
        public StateVector TranslateGameImage(GameStateImage image)
        {

            StateVector vector = new StateVector();
            int [,] towers = new int[3,3];
            
            for (var i = 0; i < image.Ranges.GetLength(0); i++)
            {
                //check this, should skip empty tower places
                if (image.Towers[i] == -1) continue;
                int j = image.Towers[i];
               
                    if (image.Ranges[i, j] < IntelligentActionInterpreter.SmallRangePathCoverage)
                    {
                        towers[j, 0]++;
                    }
                    else
                    {
                        if (image.Ranges[i, j] < IntelligentActionInterpreter.MediumRangeCoverage)
                        {
                            towers[j, 1]++;
                        }
                        else
                        {
                            towers[j, 2]++;
                        }
                    }
                
            }

            int typeOfWave = (int) image.NextWaveType;
            
            int hpPoolOfWave = image.NextWaveHpPool;

            int pointer = 0;

           /* Console.WriteLine(" -- ");
            for (int i = 0; i < image.Towers.Length; i++)
            {
                Console.Write(image.Towers[i]+" ");
            }
            Console.WriteLine();*/
            for (int i = 0; i < towers.GetLength(0); i++)
            {
                for (int j = 0; j < towers.GetLength(1); j++)
                {
                    vector[pointer++] = towers[i, j];
                  //  Console.Write(towers[i,j]+" ");

                }
            }
          /*  Console.WriteLine();
            Console.WriteLine();*/

            //type of enemies challenged
            vector[pointer++] = typeOfWave;

            //wave size // how dangerous it is
            vector[pointer++] = encodeWaveSize(image.NextWaveNumberOfEnemies, image.NextWaveEnemiesID);


            //  vector[pointer++] = hpPoolOfWave;
              vector[pointer++] = image.Level;
          //  Console.WriteLine(pointer-1);

            return vector;
        }

        public int encodeWaveSize(int size, int enemyType)
        {
            if (enemyType == 0)
            {
                if (size < 5) return 0;
                if (size < 20) return 1;
                return 2;
            }

            if (enemyType == 0)
            {
                if (size < 20) return 0;
                if (size < 70) return 1;
                return 2;
            }

            if (size < 10) return 0;
            if (size < 30) return 1;
            return 2;
        }
    }
}