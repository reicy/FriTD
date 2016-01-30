using TD.Core;
using TD.Enums;
using TD.Helpers;

namespace TD.Entities
{
    public class Projectile:IDisplayableObject
    {

        public int X { get; set; }
        public int Y { get; set; }
        public int Id { get; set; }
        public double Perc { get; set; }
        public ProjectileState State { get; private set; }
        private readonly int _speed;
        private readonly Enemy _target;
        private Effect _effect;

        public Projectile(int speed, Enemy target, Effect effect)
        {
            State = ProjectileState.Active;
            _speed = speed;
            _target = target;
            _effect = effect;
        }

        public bool IsEmpty()
        {
            return State == ProjectileState.NonActive;
        }

        public void Move()
        {
            if (ProjectileState.NonActive == State) return;
            int nextX, nextY;

            if (_target.X != X)
            {
                if (_target.X < X)
                {
                    nextX = X - _speed;
                }
                else
                {
                    nextX = X + _speed;
                }
            }
            else
            {
                nextX = X;
            }
            if (_target.Y != Y)
            {
                if (_target.Y < Y)
                {
                    nextY = Y - _speed;
                }
                else
                {
                    nextY = Y + _speed;
                }
            }
            else
            {
                nextY = X;
            }

            //MCH
            if (MathHelper.DistanceBetweenPoints(X, Y, _target.X, _target.Y) < _speed)
            {
                _target.ApplyEffect(_effect);
                _effect = null;
                State = ProjectileState.NonActive;

            };

            X = nextX;
            Y = nextY;

        }

      
    }
}