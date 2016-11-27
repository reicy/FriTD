using System.Runtime.Remoting.Contexts;
using TD.Core;

namespace Manager.Core
{
    [Synchronization]
    public class DataStore
    {
        private GameVisualImage _img;

        public GameVisualImage ExchangeData(GameVisualImage img)
        {
            if (img != null) _img = img;
            return _img;
        }
    }
}
