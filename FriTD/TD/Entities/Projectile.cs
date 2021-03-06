﻿using TD.Core;
using TD.Enums;
using TD.Helpers;

namespace TD.Entities
{
    public class Projectile : IDisplayableObject
    {
        private static int _nextSeqId; // is zero by default

        private readonly int _speed;
        private readonly Enemy _target;
        private Effect _effect;

        public int X { get; set; }
        public int Y { get; set; }
        public int Id { get; set; }
        public int SeqId { get; set; }
        public double Perc { get; set; }
        public int WX { get; set; }
        public int WY { get; set; }
        public ProjectileState State { get; private set; }
        public TDGame Game { get; set; }

        public Projectile(int speed, Enemy target, Effect effect)
        {
            State = ProjectileState.Active;
            _speed = speed;
            _target = target;
            _effect = effect;
            SeqId = _nextSeqId++;
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
                nextY = Y;
            }

            //MCH
            if (MathHelper.DistanceBetweenPoints(X, Y, _target.X, _target.Y) < _speed)
            {
                _target.ApplyEffect(_effect);

                if (_effect.IsSplash())
                {
                    GlobalEventHandler.AreaOfDmg(_target.X, _target.Y, _effect.PrimaryDmg, _effect.DmgType, _effect.SplashRadius, Game);
                }

                _effect = null;
                State = ProjectileState.NonActive;
                X = 0;
                Y = 0;
                return;
            }

            X = nextX;
            Y = nextY;
        }
    }
}
