using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manager.QLearning;

namespace Manager.AI
{
    class Action:QAction
    {

        public Int16 IntState { get; }
        public static State InitialState = new State(0);

        public Action(Int16 intState)
        {
            this.IntState = intState;
        }

        public override string ToString()
        {
            string result = "";
            string converted = Convert.ToString(IntState, 2);
            for (int i = 0; i < 15 - converted.Length; i++)
            {
                result += "0";
            }
            result += converted;

            return result;
        }

        protected bool Equals(Action other)
        {
            return IntState == other.IntState;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Action)obj);
        }

        public override int GetHashCode()
        {
            return IntState.GetHashCode();
        }

      
    }
}
