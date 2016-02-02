using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TD.Entities;
using TD.Enums;

namespace TD.Core
{
    public class TDGame
    {

        private Spawner _spawner;
        private MapBuilder _mapBuilder;
        private EnemyFactory _enemyFactory;
        private TowerFactory _towerFactory;
        private char[,] _map;
        private List<TowerPlace> _towerPlaces;
        private List<Enemy> _enemies;
        private List<Projectile> _projectiles;
        private PathSquare _spawn;

        public int Money
        {
            get { return _money; }
            private set
            {
                _money = value;
                //max 3 veze 
                if (_money > 60) _money = 60;
            }
        }

        private int _life;
        private int _money;

        public void InitGame(string towers, string map, string enemies, string levels)
        {

            _mapBuilder = new MapBuilder();
            _mapBuilder.Build(map);
            _mapBuilder.Info(out _spawn, out _map, out _towerPlaces);
            _enemyFactory = new EnemyFactory();
            _enemyFactory.InitBuilders(enemies, _spawn.Next, _spawn);
            _towerFactory = new TowerFactory();
            _towerFactory.InitBuilders(towers);
            _spawner = new Spawner(levels,_enemyFactory);
            _life = 10000;
            Money = 20;
            Enemies = new List<Enemy>();
            Projectiles = new List<Projectile>();
            State = GameState.Waiting;
            GlobalEventHandler.SetManagedGame(this);
        }

        public void StartLevel()
        {
            if (State == GameState.Waiting)
            {
                State = GameState.InProgress;
                _spawner.LoadNextWave();
            }
            
        }

        public void Tic()
        {
            Projectile proj;
            var emptyOnes = new List<Projectile>();
            var fallenOnes = new List<Enemy>();
            var victoriousOnes = new List<Enemy>();

            Enemy enemy = _spawner.Spawn();
            if (enemy != null)
            {
                Enemies.Add(enemy);
            }



            foreach (var towerPlace in _towerPlaces)
            {
                if (towerPlace.HasTower())
                {
                    proj = towerPlace.Tower().TryToFireAtFirstEnemyInSight(Enemies);
                    if (proj != null) _projectiles.Add(proj);
                }

            }


            foreach (Projectile projectile in _projectiles)
            {
                projectile.Move();
                if (projectile.IsEmpty()) emptyOnes.Add(projectile);
            }


            foreach (Enemy foe in _enemies)
            {
                foe.Move();
                if (foe.IsDead()) fallenOnes.Add(foe);
                if (foe.IsVictorious()) victoriousOnes.Add(foe);
            }



            foreach (Enemy foe in fallenOnes)
            {
                Enemies.Remove(foe);
                Money += foe.Gold;
            }


            foreach (Projectile projectile in emptyOnes)
            {
                _projectiles.Remove(projectile);
            }

            foreach (Enemy foe in victoriousOnes)
            {
                
                int moneyDmgBonus = (int)Math.Round((1.0 - foe.Hp / foe.MaxHp) * foe.Gold);
                int lifeReduction = (int)Math.Round((foe.Hp / foe.MaxHp) * foe.HpCost);
                Money += moneyDmgBonus;
                _life -= lifeReduction;
                _enemies.Remove(foe);
            }



            CheckWhetherGameStateChanged();


        }

        private void CheckWhetherGameStateChanged()
        {
            if (_life <= 0)
            {
                State = GameState.Lost;
                return;
            }
            if (_spawner.HasNoMoreInThisWave() && Enemies.Count == 0)
            {

                State = GameState.Waiting;
                if (_spawner.HasNoMore())
                {
                    State = GameState.Won;
                }
                Projectiles.Clear();
            }

        }


        public void BuildTower(int placeId, int towerId)
        {
            if (!TowerPlaces[placeId].HasTower())
            {
                var cost = _towerFactory.Cost(towerId);
                if (Money >= cost)
                {
                    Money -= cost;
                    TowerPlaces[placeId].BuildTower(_towerFactory.CreateTower(towerId));
                }
            }
        }

        public void SellTower(int placeId)
        {
            if (TowerPlaces[placeId].HasTower())
            {
                Money += _towerFactory.RefundCost(TowerPlaces[placeId].Tower().Id);
                TowerPlaces[placeId].DestroyTower();
            }
        }

        public GameState State { get; private set; }

        public char[,] Map
        {
            get { return _map; }
            set { _map = value; }
        }

        public List<TowerPlace> TowerPlaces
        {
            get { return _towerPlaces; }
            set { _towerPlaces = value; }
        }

        public List<Projectile> Projectiles
        {
            get { return _projectiles; }
            set { _projectiles = value; }
        }

        public List<Enemy> Enemies
        {
            get { return _enemies; }
            set { _enemies = value; }
        }

        public PathSquare Spawn
        {
            get { return _spawn; }
            set { _spawn = value; }
        }

        public GameStateImage GameStateImage()
        {
            var towers = new int[TowerPlaces.Count];
            //-1 no tower placed, 0 - 2 tower ids
            for (int i = 0; i < TowerPlaces.Count; i++)
            {
                towers[i] = -1;
                if (TowerPlaces[i].HasTower())
                {
                    towers[i] = TowerPlaces[i].Tower().Id;
                }
            }

            var img = new GameStateImage()
            {
                Gold = Money,
                Hp = _life,
                NextWaveHpCost = _spawner.NextWaveHpCost(),
                Towers = towers,
                TowerCost = _towerFactory.Cost(0),
                TowerRefundCost = _towerFactory.RefundCost(0)
            };

            return img;
        }

        public GameVisualImage GameVisualImage()
        {
            var foes = Enemies.Cast<IDisplayableObject>().ToList();
            var shots = Projectiles.Cast<IDisplayableObject>().ToList();
            var towers = (from towerPlace in TowerPlaces where towerPlace.HasTower() select towerPlace.Tower()).Cast<IDisplayableObject>().ToList();


            var img = new GameVisualImage()
            {
                Gold = Money,
                Hp = _life,
                NextWaveHpCost = _spawner.NextWaveHpCost(),
                TowerCost = _towerFactory.Cost(0),
                TowerRefundCost = _towerFactory.RefundCost(0),
                Map = Map,
                Enemies = foes,
                Towers = towers,
                Projectiles = shots
            };



            return img;
        }


    }

    
}
