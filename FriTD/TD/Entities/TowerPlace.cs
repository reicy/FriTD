namespace TD.Entities
{
    public class TowerPlace
    {
        private readonly int[] _fieldsInRangePerTowerType;
        private Tower _tower;

        public int X { get; set; }
        public int Y { get; set; }

        public TowerPlace()
        {
            _fieldsInRangePerTowerType = new int[20];
        }

        public bool HasTower()
        {
            return _tower != null;
        }

        public void BuildTower(Tower tower)
        {
            tower.X = X;
            tower.Y = Y;
            _tower = tower;
        }

        public void DestroyTower()
        {
            _tower = null;
        }

        public Tower Tower()
        {
            return _tower;
        }

        public void TypePathFieldInRange(int id)
        {
            _fieldsInRangePerTowerType[id]++;
        }

        public int TypePathFieldsInRangeCount(int id)
        {
            return _fieldsInRangePerTowerType[id];
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < 3; i++)
            {
                s += " " + _fieldsInRangePerTowerType[i];
            }
            return s;
        }
    }
}
