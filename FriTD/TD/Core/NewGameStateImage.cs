using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TD.Entities;
using TD.Enums;

namespace TD.Core
{
    public class NewGameStateImage
    {
        public int Hp { get; set; }
        public GameState GameState { get; set; }
        public List<Tower> Towers { get; set; }  // veze ktore su postavene


        public int NextWaveHpCost { get; set; } // ???????
        public int Gold { get; set; } //???? bude v stave???
        public int TowerRefundCost { get; set; }
        public int TowerCost { get; set; } // ????? co to je
       
        public NewGameStateImage CloneThis()
        {

            return new NewGameStateImage()
            {
                Hp = Hp,
                GameState = GameState,
                Towers = new List<Tower>(Towers),

                Gold = Gold, //??

                NextWaveHpCost = NextWaveHpCost,
                TowerRefundCost = TowerRefundCost,
                TowerCost = TowerCost
            };
        }
    }
}
