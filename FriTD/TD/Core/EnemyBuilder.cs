using TD.Entities;

namespace TD.Core
{
    class EnemyBuilder
    {

        public string Template { get; }
        public PathSquare WayPoint { get; set; }
        public PathSquare Spawn { get; set; }
        public int HpCost { get; set; }

        private int _cd;
        private int _remainingCd;

        public EnemyBuilder(string template, PathSquare wayPoint, PathSquare spawn)
        {
            Spawn = spawn;
            WayPoint = wayPoint;
            Template = template;
            _cd = 0;
            _remainingCd = 0;
        }

        public Enemy Build()
        {
            if (_remainingCd > 0)
            {
                _remainingCd--;
                return null;
            }

            var temp =Template.Split(',');
            _cd = int.Parse(temp[8]);
            _remainingCd = _cd;
            HpCost = int.Parse(temp[5]);

            return new Enemy()
            {
                Hp = int.Parse(temp[2]),
                MaxHp = int.Parse(temp[2]),
                Speed = int.Parse(temp[3]),
                MaxSpeed = int.Parse(temp[3]),
                Gold = int.Parse(temp[4]),
                HpCost = int.Parse(temp[5]),
                Armor = int.Parse(temp[6]),
                MagicResist = int.Parse(temp[7]),
                SquareWayPoint = WayPoint,
                X = Spawn.X,
                Y = Spawn.Y

            };


        }

      
    }
}
