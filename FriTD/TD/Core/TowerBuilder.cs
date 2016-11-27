using TD.Entities;
using TD.Enums;
using TD.Helpers;

namespace TD.Core
{
    public class TowerBuilder
    {
        public string Template { get; }
        public int Cost { get; }
        public int Refund { get; }
        public int Cd { get; }
        public int Range { get; }
        public int ProjSpeed { get; }
        public int Id { set; get; }
        public TDGame Game { get; set; }

        private readonly ProjectileEffectFactory _factory;

        public TowerBuilder(string template)
        {
            Template = template;
            var arr = Template.Split(',');
            Cost = int.Parse(arr[1]);
            Refund = int.Parse(arr[2]);
            Cd = int.Parse(arr[5]);
            Range = int.Parse(arr[6]);
            ProjSpeed = int.Parse(arr[12]);

            _factory = new ProjectileEffectFactory
            {
                SecondaryDmg = int.Parse(arr[9]),
                PrimaryDmg = int.Parse(arr[4]),
                Slow = int.Parse(arr[7]),
                RemainingTurns = int.Parse(arr[8]),
                DmgType = (DmgType)int.Parse(arr[3]),
                Splash = int.Parse(arr[10]),
                SplashRadius = int.Parse(arr[11]),
                Cd = Cd
            };
        }

        public Tower Build()
        {
            return new Tower(Range, _factory, ProjSpeed) { Game = Game };
        }

        public void EvaluateTowerPlace(TowerPlace towerPlace, char[,] map)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 'P')
                    {
                        if (
                            MathHelper.DistanceBetweenPoints((j + 0.5) * MapBuilder.SQUARE_SIZE, (i + 0.5) * MapBuilder.SQUARE_SIZE, towerPlace.X, towerPlace.Y) <=
                            Range)
                        {
                            towerPlace.TypePathFieldInRange(Id);
                        }
                    }
                }
            }
        }
    }
}
