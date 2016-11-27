using System.Collections.Generic;
using System.IO;
using System.Linq;
using TD.Entities;
using TD.Enums;

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

            using (var reader = new StringReader(levels))
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
                return 0;

            string current = _levelTemplates.ElementAt(0);
            var arr = current.Split(':');
            int type = int.Parse(arr[0]);
            int count = int.Parse(arr[1]);
            return count * _enemyFactory.HpCost(type);
        }

        public EnemyType NextWaveType()
        {
            if (_levelTemplates.Count == 0)
                return EnemyType.Unknown;

            string current = _levelTemplates.ElementAt(0);
            var arr = current.Split(':');
            int id = int.Parse(arr[0]);

            string type = _enemyFactory.EnemyType(id);
            switch (type)
            {
                case "heavy": return EnemyType.Heavy;
                case "swarm": return EnemyType.Swarm;
            }

            return EnemyType.Unknown;
        }

        public int NextWaveHpPool()
        {
            if (_levelTemplates.Count == 0) return 0;
            string current = _levelTemplates.ElementAt(0);

            var arr = current.Split(':');
            int id = int.Parse(arr[0]);
            int count = int.Parse(arr[1]);
            int hp = _enemyFactory.EnemyHp(id);

            return count * hp;
        }

        public int NextWaveEnemiesNum()
        {
            if (_levelTemplates.Count == 0) return 0;
            string current = _levelTemplates.ElementAt(0);

            var arr = current.Split(':');
            int count = int.Parse(arr[1]);
            return count;
        }

        public int NextWaveEnemiesId()
        {
            if (_levelTemplates.Count == 0) return 0;
            string current = _levelTemplates.ElementAt(0);

            var arr = current.Split(':');
            int id = int.Parse(arr[0]);

            return id;
        }
    }
}
