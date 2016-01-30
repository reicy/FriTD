using System.Collections.Generic;
using System.Diagnostics;
using TD.Core;
using TD.Enums;
using TD.Helpers;

namespace TD.Entities
{
    public class Enemy:IDisplayableObject
    {
        public string Name { get; set; }
        public EnemyCategory Category { get; set; }
        public double Hp { get; set; }
        public double MaxHp { get; set; }
        public int Speed { get; set; }
        public int MaxSpeed { get; set; }
        public int Gold { get; set; }
        public int HpCost { get; set; }
        public int Armor { get; set; }
        public int MagicResist { get; set; }
        public int SpawnCd { get; set; }
        public EnemyState State { get; set; }
        private readonly LinkedList<Effect> _effects;
        public PathSquare SquareWayPoint { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Id { get; set; }
        public double Perc
        {
            get {return Hp/MaxHp;}
            set { Perc = value; }
        }

        public Enemy()
        {
            _effects = new LinkedList<Effect>();
        }


        public void Move()
        {
            Debug.WriteLine("Enemy hp: "+Hp+" type "+Id+ " moved from "+Y+" "+X+" "+Speed);
            
            var expiredEffects = new LinkedList<Effect>();
            Speed = MaxSpeed;

            //add here projectile effect application
            foreach (var effect in _effects)
            {
                Speed = (int)(Speed * ((100-effect.Slow)/100));
                Hp -= effect.Dmg(Armor, MagicResist);
                effect.ReduceTtl();
                if (effect.RemainingTurns <= 0) expiredEffects.AddLast(effect);
            }
            if (Hp <= 0)
            {
                State = EnemyState.Dead;

            }
            foreach (var effect in expiredEffects)
            {
                _effects.Remove(effect);
            }

            if (State == EnemyState.Dead) return;


            if (SquareWayPoint == null)
            {
                State = EnemyState.Victorious;
                return;
            }


            int nextX, nextY;

            if (SquareWayPoint.X != X)
            {
                if (SquareWayPoint.X < X)
                {
                    nextX = X - Speed;
                }
                else
                {
                    nextX = X + Speed;
                }
            }
            else
            {
                nextX = X;
            }
            if (SquareWayPoint.Y != Y)
            {
                if (SquareWayPoint.Y < Y)
                {
                    nextY = Y - Speed;
                }
                else
                {
                    nextY = Y + Speed;
                }
            }
            else
            {
                nextY = Y;
            }
            Debug.WriteLine("Enemy hp: " + Hp + " type " + Id + " moved to " + Y + " " + X);
            Debug.WriteLine("Way point "+SquareWayPoint.X +" "+SquareWayPoint.Y);
            //MCH
            if (MathHelper.DistanceBetweenPoints(nextX, nextY, SquareWayPoint.X, SquareWayPoint.Y) < 2 * Speed)
            {
                SquareWayPoint = SquareWayPoint.Next;

            }

            X = nextX;
            Y = nextY;

        }

        public bool IsDead()
        {

            return State == EnemyState.Dead;
        }
        

        public bool IsVictorious()
        {
            return State == EnemyState.Victorious;
        }

        




        public void ApplyEffect(Effect effect)
        {
            _effects.AddLast(effect);
        }

        
    }
}