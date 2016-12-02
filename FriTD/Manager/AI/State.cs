using System;
using Manager.QLearning;

namespace Manager.AI
{
    public class State : QState
    {
        public short IntState { get; private set; }
        public static State InitialState = new State(0);

        public State() : this(0)
        {
        }

        public State(short intState)
        {
            IntState = intState;
        }

        public override string ToString()
        {
            var converted = Convert.ToString(IntState, 2);
            var result = new string('0', 15 - converted.Length);
            result += converted;

            return result;
        }

        public void FromString(string str)
        {
            IntState = Convert.ToInt16(str, 2);
        }

        protected bool Equals(State other)
        {
            return IntState == other.IntState;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((State)obj);
        }

        public override int GetHashCode()
        {
            return IntState.GetHashCode();
        }
    }
}
