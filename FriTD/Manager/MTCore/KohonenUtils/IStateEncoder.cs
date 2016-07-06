using Manager.Kohonen;
using TD.Core;

namespace Manager.MTCore.KohonenUtils
{
    public interface IStateEncoder
    {
        StateVector TranslateGameImage(GameStateImage image);
    }
}