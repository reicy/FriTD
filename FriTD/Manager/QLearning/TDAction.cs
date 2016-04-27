using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manager.QLearning
{
    class TDAction : QAction
    {
        double actionCost;
        int actionId;

        //TODO
        public TDAction(int actionId, double actionCost)
        {
            this.actionCost = actionId;
            this.actionCost = actionCost;
        }

        //TODO
        public QState getNewState(QState prevState)
        {
            switch (actionId)
            {
                case 0: return prevState;
                case 1: return null;//





                default: return prevState;
    

            }


            return null;
        }


    }
}
