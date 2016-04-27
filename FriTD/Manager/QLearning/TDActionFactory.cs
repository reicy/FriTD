using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manager.QLearning
{
    class TDActionFactory
    {
        static List<TDAction> actions;
        static TDActionFactory(){
            //id 0 = no action
            actions.Add(new TDAction(1,10));
            actions.Add(new TDAction(2,10));
            actions.Add(new TDAction(3,10));
            actions.Add(new TDAction(4,10));
            actions.Add(new TDAction(5,10));
            // TODO ....................

        }
        //TODO
        public static TDAction getAction(int actionId)
        {
            return actions[actionId];
        }

        //TODO
        public static List<TDAction> getPossibleActions(double golds)
        {


            return null;
        }
    }
}
