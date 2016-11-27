using TD.Entities;
using TD.Enums;
using TD.Helpers;

namespace TD.Core
{
    public static class GlobalEventHandler
    {
        public static void AreaOfDmg(int x, int y, double dmg, DmgType dmgType, int radius, TDGame game)
        {
            foreach (var enemy in game.Enemies)
            {
                if (MathHelper.DistanceBetweenPoints(enemy.X, enemy.Y, x, y) <= radius)
                {
                    enemy.ApplyEffect(new Effect { DmgType = dmgType, PrimaryDmg = dmg, RemainingTurns = 1, SecondaryDmg = 0, Slow = 0, Splash = 0, SplashRadius = 0 });
                }
            }
        }
    }
}
