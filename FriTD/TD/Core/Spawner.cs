using System.Collections.Generic;
using System.IO;
using System.Linq;
using TD.Entities;

namespace TD.Core
{
    public class Spawner
    {
        private readonly EnemyFactory _enemyFactory;
        private readonly LinkedList<string> _levelTemplates;
        private int _type;
        private int _remaining;

        public Spawner(string levels, EnemyFactory enemyFactory)
        {
            _enemyFactory = enemyFactory;
            _levelTemplates = new LinkedList<string>();
            _type = 0;
            _remaining = 0;
            using (StreamReader reader = new StreamReader(levels))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    _levelTemplates.AddLast(line);
                }

            }

        }



        public void LoadNextWave()
        {
            string current = _levelTemplates.ElementAt(0);
            _levelTemplates.RemoveFirst();
            var arr = current.Split(':');
            _type = int.Parse(arr[0]);
            _remaining = int.Parse(arr[1]);
        }

        public Enemy Spawn()
        {
            if (_remaining <= 0) return null;
            var enemy = _enemyFactory.CreateEnemy(_type);
            if (enemy != null) _remaining--;
            return enemy;
            
        }

        public bool HasNoMoreInThisWave()
        {
            return _remaining <= 0;
        }

        public bool HasNoMore()
        {
            return _remaining <= 0 && _levelTemplates.Count == 0;
        }

        public int NextWaveHpCost()
        {
            if (_levelTemplates.Count == 0)
            {
                return 0;
            }
            string current = _levelTemplates.ElementAt(0);
            var arr = current.Split(':');
            int type = int.Parse(arr[0]);
            int count = int.Parse(arr[1]);
            return count*_enemyFactory.HpCost(type);
        }
    }
}