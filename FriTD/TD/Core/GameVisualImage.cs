using System.Collections.Generic;
using TD.Enums;

namespace TD.Core
{
    public class GameVisualImage
    {
        public int Hp { get; set; }
        public int NextWaveHpCost { get; set; }
        public int Gold { get; set; }
        public int TowerRefundCost { get; set; }
        public int TowerCost { get; set; }
        public GameState State { get; set; }

        public List<IDisplayableObject> Towers { get; set; }
        public List<IDisplayableObject> Projectiles { get; set; }
        public List<IDisplayableObject> Enemies { get; set; }
        public char[,] Map { get; set; }

    }
}