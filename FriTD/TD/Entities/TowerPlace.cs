using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD.Entities
{
    public class TowerPlace
    {
        public int X { get; set; }
        public int Y { get; set; }

        private Tower _tower;

        public bool HasTower()
        {
            return _tower != null;
        }

        public void BuildTower(Tower tower)
        {
            tower.X = X;
            tower.Y = Y;
            this._tower = tower;
        }

        public void DestroyTower()
        {
            _tower = null;
        }

        public Tower Tower()
        {
            return _tower;
        }
       




    }
}
