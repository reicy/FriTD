
using System;
using System.Collections.Generic;

namespace Manager.AI
{
    class AICore
    {
        double epsilon, gamma, alpha;

        Dictionary<State, Dictionary<State, double>> q_values; 

        public AICore(double e,double g,double a)
        {
            alpha = a;
            epsilon = e;
            gamma = g;
            q_values = new Dictionary<State, Dictionary<State, double>>();
        }


        public void QValDisp()
        {
            Console.WriteLine("hej");
            Console.WriteLine(q_values.Keys.Count);
            foreach (var key in q_values.Keys)
            {
                Console.Write(key.toString()+" - ");
                foreach (var innerKey in q_values[key].Keys)
                {
                    //Console.WriteLine(q_values[key].Keys.Count);
                   // Console.Write(" {0} v:{1}",innerKey.toString(),(q_values[key])[innerKey]);
                    Console.WriteLine(innerKey.toString()+"    "+q_values[key][innerKey]);
                }
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        //update q_Value of a pair state-state(action) after executing and getting the reward
        public void updateQ_values(State prevState,State nextState,double reward)
        {

            double maxNextStateValue = int.MinValue;
            State maxNextState;
            createAction(prevState, nextState);
            Dictionary<State, double> StatesFromAState;// = q_values[prevState];
            if (q_values.TryGetValue(prevState, out StatesFromAState))
            {

                foreach (KeyValuePair<State, double> entry in StatesFromAState)
                {
                    if (entry.Value >= maxNextStateValue)
                    {
                        maxNextStateValue = entry.Value;
                        maxNextState = entry.Key;
                    }
                }
                double sample = reward + gamma*maxNextStateValue;
              //  Console.WriteLine("r "+sample+ " "+reward +" "+gamma*maxNextStateValue);
                double prevQ_value = q_values[prevState][nextState];
                q_values[prevState][nextState] = (1 - alpha)*prevQ_value + alpha*sample;
            }




       //     Console.WriteLine(" ------ ");
       //     QValDisp();

        }

        //dynamicaly create Pair state-state if not already created
        public void createAction(State prevState, State nextState)
        {
            Dictionary<State, double> StatesFromAState; 
            double tempReward;

            if (q_values.TryGetValue(prevState, out StatesFromAState))
            {
                if (!StatesFromAState.TryGetValue(nextState, out tempReward)) {
                    StatesFromAState.Add(nextState, 0);
                }
            }
            else
            {
                StatesFromAState = new Dictionary<State, double>();
                StatesFromAState.Add(nextState, 0);
                q_values.Add(prevState, StatesFromAState);
            }
        }


        //get action with max q_value
        //!!!!! if only negative q_values it still chooses from them
        public State getNextOptimalState(State state,List<State> relevantStates)
        {
            Dictionary<State, double> StatesFromAState;
            q_values.TryGetValue(state, out StatesFromAState);
            double maxValue=double.MinValue;
            State maxState = state;
            double tempValue;

            if (StatesFromAState == null) Console.Write("Something is ***** wrong");

            foreach (State s in relevantStates)
            {
                if(StatesFromAState.TryGetValue(s,out tempValue))
                {
                    if (maxValue <= tempValue)
                    {
                        maxState = s;
                        maxValue = tempValue;
                    }
                }
            }

            return maxState;
        }

        //get random q_state
        public State getNextRandomState(State state, List<State> relevantStates)
        {
            Random rnd = new Random();
            int randomIndex = rnd.Next(relevantStates.Count);
            State nextState = relevantStates[randomIndex];
            createAction(state,nextState);

            return nextState;
        }

        //get the next action by the rules of q_learning (alpha,gamma,eps)
        public State getNextState(State state, List<State> relevantStates)
        {
            if (relevantStates == null) Console.Write("Something is ***** wrong");

            Dictionary<State, double> StatesFromAState;
            Random rnd = new Random();
            double randomDouble = rnd.NextDouble();

            if (!q_values.TryGetValue(state, out StatesFromAState)) return getNextRandomState(state, relevantStates);
            else if (randomDouble <= epsilon) return getNextRandomState(state, relevantStates);
            else return  getNextOptimalState(state, relevantStates);
        }


    }
}
