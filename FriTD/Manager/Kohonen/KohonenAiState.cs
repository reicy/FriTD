using System;
using Manager.QLearning;

namespace Manager.Kohonen
{
    public class KohonenAiState : QState
    {
        private readonly int[] _dim;

        public KohonenAiState(int[] dim)
        {
            _dim = dim;
        }

        public KohonenAiState()
        {
            _dim = new[] { 0, 0 };
        }

        protected bool Equals(KohonenAiState other)
        {
            return _dim[0] == other._dim[0] && _dim[1] == other._dim[1];
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((KohonenAiState)obj);
        }

        public override int GetHashCode()
        {
            //TODO inak
            return (_dim != null ? _dim[0] * 1000 + _dim[1] : 0);
        }

        public override string ToString()
        {
            return _dim[0] + " " + _dim[1] + "     ";
        }

        public void FromString(string str)
        {
            var tmp = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            _dim[0] = int.Parse(tmp[0]);
            _dim[1] = int.Parse(tmp[1]);
        }
    }
}
