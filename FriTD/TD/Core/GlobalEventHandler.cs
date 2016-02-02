using TD.Entities;
using TD.Enums;
using TD.Helpers;

namespace TD.Core
{
    public static class GlobalEventHandler
    {
        private static TDGame _managedGame;

        public static void SetManagedGame(TDGame game)
        {
            _managedGame = game;
        }

        public static void AreaOfDmg(int x, int y, double dmg, DmgType dmgType, int radius)
        {
            if (_managedGame == null) return;
            foreach (var enemy in _managedGame.Enemies)
            {
                if (MathHelper.DistanceBetweenPoints(enemy.X, enemy.Y, x, y) <= radius)
                {
                    enemy.ApplyEffect(new Effect() {DmgType = dmgType, PrimaryDmg = dmg, RemainingTurns = 1, SecondaryDmg = 0, Slow = 0, Splash = 0, SplashRadius = 0});
                }
            }
        }

    }
}