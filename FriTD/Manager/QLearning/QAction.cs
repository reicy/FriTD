using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manager.QLearning
{
    interface QAction
    {
        QState getNewState(QState prevState);
    }
}
