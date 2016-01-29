using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using TD.Core;

namespace Manager.Core
{
    [Synchronization]
    class DataStore
    {

        private GameVisualImage _img;

        public GameVisualImage ExchangeData(GameVisualImage img)
        {
            if (img != null) _img = img;

            return _img;
        }
       

    }
}
