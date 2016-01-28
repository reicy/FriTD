using TD.Entities;
using TD.Enums;

namespace TD.Core
{
    public class ProjectileEffectFactory
    {
        public int RemainingTurns { get; set; }
        public double PrimaryDmg { get; set; }
        public DmgType DmgType { get; set; }
        public double SecondaryDmg { get; set; }
        public double Slow { get; set; }
        public bool PrimaryDmgUsed { get; set; }
        public int Splash { get; set; }
        public int SplashRadius { get; set; }
        public int Cd { get; set; }
        private int _cdCounter;

        public ProjectileEffectFactory()
        {
            _cdCounter = 0;
        }

        public Effect Create()
        {

            if (_cdCounter > 0)
            {
                _cdCounter--;
                return null;
            }
            else
            {
                _cdCounter = Cd;
                return new Effect()
                {
                    DmgType = DmgType,
                    PrimaryDmgUsed = PrimaryDmgUsed,
                    PrimaryDmg = PrimaryDmg,
                    RemainingTurns = RemainingTurns,
                    SecondaryDmg = SecondaryDmg,
                    Slow = Slow,
                    Splash = Splash,
                    SplashRadius = SplashRadius
                };
            }

            
        }

    }
}