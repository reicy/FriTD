using System.Collections;
using System.Collections.Generic;
using System.IO;
using TD.Entities;

namespace TD.Core
{
    public class EnemyFactory
    {

        private readonly List<EnemyBuilder> _builders;

        public EnemyFactory( )
        {
            _builders = new List<EnemyBuilder>();
        }


        public void InitBuilders(string enemiesFile, PathSquare wayPoing, PathSquare spawn)
        {
            using (var reader = new StreamReader(enemiesFile))
            {
                string line;
                while ((line = reader.ReadLine())!=null)
                {
                    _builders.Add(new EnemyBuilder(line, wayPoing, spawn));
                }
                
            }
        }

        public Enemy CreateEnemy(int id)
        {

            var enemy = _builders[id].Build();
            enemy.Id = id;
            return enemy;
        }


        public int HpCost(int id)
        {
            return _builders[id].HpCost;
        }
    }
}