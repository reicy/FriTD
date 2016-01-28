using System.Collections;
using System.Collections.Generic;
using System.IO;
using TD.Entities;

namespace TD.Core
{
    public class TowerFactory
    {

        private readonly List<TowerBuilder> _builders;

        public TowerFactory( )
        {
            _builders = new List<TowerBuilder>();
        }


        public void InitBuilders(string towerFile, PathSquare wayPoing, PathSquare spawn)
        {
            using (var reader = new StreamReader(towerFile))
            {
                string line;
                while ((line = reader.ReadLine())!=null)
                {
                    _builders.Add(new TowerBuilder(line));
                }
                
            }
        }

        public Enemy CreateEnemy(int id)
        {
            return _builders[id].Build();
        }


        
    }
}