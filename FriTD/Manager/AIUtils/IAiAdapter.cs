using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TD.Core;

namespace Manager.AIUtils
{
    interface IAiAdapter
    {
        String ExecuteDecision(GameStateImg gameStateImage);
        void ExecuteReward(GameStateImg gameStateImage);
    }
}
