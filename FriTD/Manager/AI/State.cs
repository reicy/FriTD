using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI
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
            string result="";
            string converted = Convert.ToString(IntState, 2);
            for(int i = 0; i < 15-converted.Length; i++)
            {
                result += "0";
            }
            result += converted;

            return result;
        }


    }
}
