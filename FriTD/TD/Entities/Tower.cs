using System.Collections.Generic;
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
        public double Perc { get; set; }

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
                if (MathHelper.DistanceBetweenPoints(enemy.X, enemy.Y, X, Y) <= _range)
                {
                    var effect = _effectFactory.Create();
                    if (effect == null) return null;
                    projectile = new Projectile(_projSpeed, enemy, effect) {X = X, Y = Y, Id = Id};
                   
                    break;
                };
            }

            return projectile;

        }

    }
}