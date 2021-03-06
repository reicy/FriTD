﻿using TD.Enums;

namespace TD.Core
{
    public class GameStateImage : GameStateImg
    {
        public int Hp { get; set; }
        public int NextWaveHpCost { get; set; }
        public int Gold { get; set; }
        public int TowerRefundCost { get; set; }
        public int TowerCost { get; set; }
        public int[] Towers { get; set; }
        public GameState GameState { get; set; }
        public int[,] Ranges { get; set; }
        public int Level { get; set; }
        public EnemyType NextWaveType { get; set; }
        public int NextWaveHpPool { get; set; }
        public int NextWaveNumberOfEnemies { get; set; }
        public int NextWaveEnemiesId { get; set; }
        public double MaxTowers { get; set; }
        public double MaxNextWaveHpPool { get; set; }
        public double MaxType { get; set; }

        public GameStateImage CloneThis()
        {
            return new GameStateImage
            {
                Hp = Hp,
                NextWaveHpCost = NextWaveHpCost,
                Gold = Gold,
                Towers = (int[])Towers.Clone(),
                TowerRefundCost = TowerRefundCost,
                GameState = GameState,
                TowerCost = TowerCost
            };
        }
    }
}
