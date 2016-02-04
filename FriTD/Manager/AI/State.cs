using System;
namespace Manager.AI
{
    class State
    {
        private Int16 IntState { get; }

        public State(Int16 intState)
        {
            this.IntState = intState;
        }

        public string toString()
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

        protected bool Equals(State other)
        {
            return IntState == other.IntState;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((State) obj);
        }

        public override int GetHashCode()
        {
            return IntState.GetHashCode();
        }
    }

    
}
