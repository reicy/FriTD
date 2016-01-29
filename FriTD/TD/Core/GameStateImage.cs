using System.Collections.Generic;
using TD.Entities;

namespace TD.Core
{
    public class GameStateImage
    {
        public int Hp { get; set; }
        public int NextWaveHpCost { get; set; }
        public int Gold { get; set; }
        public int TowerRefundCost { get; set; }
        public int TowerCost { get; set; }
        public int[] Towers { get; set; }

    }
}