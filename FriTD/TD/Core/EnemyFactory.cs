using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TD.Entities;

namespace TD.Core
{
    public class EnemyFactory
    {

        private readonly List<EnemyBuilder> _builders;
        private int _nextSeqId;


        public EnemyFactory()
        {
            _builders = new List<EnemyBuilder>();
            _nextSeqId = 0;
        }


        public void InitBuilders(string enemiesFile, PathSquare wayPoing, PathSquare spawn)
        {
            using (var reader = new StringReader(enemiesFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    //Debug.WriteLine(line);
                    _builders.Add(new EnemyBuilder(line, wayPoing, spawn));
                }

            }
        }

        public Enemy CreateEnemy(int id)
        {

            var enemy = _builders[id].Build();
            if (enemy != null)
            {
                enemy.Id = id;
                enemy.SeqId = _nextSeqId++;
            }
            return enemy;
        }


        public int HpCost(int id)
        {
            //Debug.WriteLine(id);
            return _builders[id].HpCost;
        }

        public string EnemyType(int id)
        {
            return _builders[id].EnemyType();
        }

        public int EnemyHp(int id)
        {
            return _builders[id].EnemyHp();
        }
    }
}