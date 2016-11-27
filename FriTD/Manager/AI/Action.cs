using Manager.QLearning;

namespace Manager.AI
{
    class Action : QAction
    {
        public int IntState { get; }
        public static State InitialState = new State(0);

        public Action(int intState)
        {
            IntState = intState;
        }

        public override string ToString()
        {
            /*string converted = Convert.ToString(IntState, 2);
            var result = new string('0', 15 - converted.Length);
            result += converted;*/
            var result = "" + IntState;

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
