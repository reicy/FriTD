using System.Collections.Generic;
using System.Diagnostics;
using System.Security.AccessControl;
using TD.Core;
using TD.Helpers;

namespace TD.Entities
{
    public class Tower: IDisplayableObject
    {
        private readonly int _range;
        private readonly ProjectileEffectFactory _effectFactory;
        private readonly int _projSpeed;
        public int X { get; set; }
        public int Y { get; set; }
        public int Id { get; set; }
        public int SeqId { get; set; }
        public double Perc { get; set; }
        public TDGame Game { get; set; }
        public int WX
        {
            get { return _lastEnemy != null ?_lastEnemy.X : 0; }
            set { }
        }

        public int WY {
            get { return _lastEnemy != null ? _lastEnemy.Y : 0; }
            set { }
        }
        private Enemy _lastEnemy;

        public Tower(int range, ProjectileEffectFactory eFactory, int ProjSpeed)
        {
            _range = range;
            _effectFactory = eFactory;
            _projSpeed = ProjSpeed;
        }

       

        public Projectile TryToFireAtFirstEnemyInSight(List<Enemy> enemies)
        {
            

            Projectile projectile = null;
            foreach (Enemy enemy in enemies)
            {
               // Debug.WriteLine("Range: "+_range);
                if (MathHelper.DistanceBetweenPoints(enemy.X, enemy.Y, X, Y) <= _range)
                {
                   // Debug.WriteLine("Firing "+SeqId);
                    var effect = _effectFactory.Create();
                    if (effect == null) return null;
                    projectile = new Projectile(_projSpeed, enemy, effect) {X = X, Y = Y, Id = Id, Game = Game};
                    _lastEnemy = enemy;
                    
                    break;
                };
            }

            return projectile;

        }

        
    }
}