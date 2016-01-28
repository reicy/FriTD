using TD.Enums;

namespace TD.Entities
{
    public class Effect
    {

        public int RemainingTurns { get; set; }
        public double PrimaryDmg { get; set; }
        public DmgType DmgType { get; set; }
        public double SecondaryDmg { get; set; }
        public double Slow { get; set; }
        public bool PrimaryDmgUsed { get; set; }
        public int Splash { get; set; }
        public int SplashRadius { get; set; }

        public double Dmg(int armor, int magicResist)
        {
            //TODO splash here
            var dmg = PrimaryDmgUsed ? SecondaryDmg : PrimaryDmg;
            PrimaryDmgUsed = true;
            var decrease = DmgType == DmgType.Magical ? magicResist : armor;
            return dmg*((100 - decrease)*1.0/100);
        }

        public void ReduceTtl()
        {
            RemainingTurns--;
        }

       
    }
}
