using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TD.Entities;

namespace TD.Core
{
    public class TowerFactory
    {

        private readonly List<TowerBuilder> _builders;
        private int _nextSeqId;
        public TDGame Game { get; set; }


        public TowerFactory( )
        {
            _builders = new List<TowerBuilder>();
            _nextSeqId = 0;
        }


        public void InitBuilders(string towerFile)
        {
            using (var reader = new StringReader(towerFile))
            {
                string line;
                
                while ((line = reader.ReadLine())!=null)
                {
                    var towerBuilder = new TowerBuilder(line) {Id = _builders.Count, Game = Game};
                    _builders.Add(towerBuilder);
                }
                
            }
        }

        

        public Tower CreateTower(int id)
        {
            var tower = _builders[id].Build();
            tower.Id = id;
            tower.SeqId = _nextSeqId++;
            return tower;
        }

        public int RefundCost(int id)
        {
            return _builders[id].Refund;
        }

        public int Cost(int id)
        {
            return _builders[id].Cost;
        }

        public void EvalTowerPlaces(List<TowerPlace> towerPlaces, char[,] map)
        {
            foreach (var towerPlace in towerPlaces)
            {
                foreach (var towerBuilder in _builders)
                {
                    
                    towerBuilder.EvaluateTowerPlace(towerPlace, map);
                }
            }
        }

        public int TowerTypesCount()
        {
            return _builders.Count;
        }
    }
}