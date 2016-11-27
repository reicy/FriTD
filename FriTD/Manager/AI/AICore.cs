using System;
using System.Collections.Generic;
using System.IO;

namespace Manager.AI
{
    class AiCore
    {
        readonly double _epsilon, _gamma, _alpha;
        int _iterationCounter;
        int _iterationsToSave = 100;
        readonly Dictionary<State, Dictionary<State, double>> _qValues;

        public AiCore(double epsilon, double gamma, double alpha)
        {
            _alpha = alpha;
            _epsilon = epsilon;
            _gamma = gamma;
            _qValues = new Dictionary<State, Dictionary<State, double>>();
            _iterationCounter = 0;
        }

        public AiCore(double epsilon, double gamma, double alpha, StreamReader sr)
        {
            _alpha = alpha;
            _epsilon = epsilon;
            _gamma = gamma;
            _qValues = new Dictionary<State, Dictionary<State, double>>();
            _iterationCounter = 0;

            //readQ_valuesFromFile(sr);

            //TODO
        }

        public void QValDisp()
        {
            //Console.WriteLine("hej");
            Console.WriteLine(_qValues.Keys.Count);
            foreach (var key in _qValues.Keys)
            {
                Console.Write(@"{0} - ", key);
                foreach (var innerKey in _qValues[key].Keys)
                {
                    //Console.WriteLine(_qValues[key].Keys.Count);
                    //Console.Write(" {0} v:{1}", innerKey, _qValues[key][innerKey]);
                    Console.Write(@"{0}    {1}", innerKey, _qValues[key][innerKey]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            //Console.WriteLine();
        }

        //update q_Value of a pair state-state(action) after executing and getting the reward
        public void updateQ_values1(State prevState, State nextState, State action, double reward)
        {
            double maxNextStateValue = int.MinValue;
            CreateAction(prevState, nextState);
            Dictionary<State, double> statesFromAState; // = _qValues[prevState];

            if (_qValues.TryGetValue(prevState, out statesFromAState))
            {
                foreach (KeyValuePair<State, double> entry in statesFromAState)
                {
                    if (entry.Value >= maxNextStateValue)
                    {
                        maxNextStateValue = entry.Value;
                    }
                }
                double sample = reward + _gamma * maxNextStateValue;
                //Console.WriteLine(@"r {0} {1} {2}", sample, reward, gamma * maxNextStateValue);
                double prevQValue = _qValues[prevState][nextState];
                _qValues[prevState][nextState] = (1 - _alpha) * prevQValue + _alpha * sample;
            }

            //Console.WriteLine(" ------ ");
            //QValDisp();
        }

        public void updateQ_values(State prevState, State nextState, State action, double reward)
        {
            if (prevState.Equals(State.InitialState)) _iterationCounter++;
            if (_iterationCounter % _iterationsToSave == 0)
            {
                //saveQ_valuesToFile(string.Format("{0}.txt", _iterationCounter));
            }

            double maxNextStateValue = int.MinValue;
            CreateAction(prevState, action);
            Dictionary<State, double> statesFromAState; // = _qValues[prevState];
            if (_qValues.TryGetValue(nextState, out statesFromAState))
            {
                foreach (KeyValuePair<State, double> entry in statesFromAState)
                {
                    if (entry.Value >= maxNextStateValue)
                    {
                        maxNextStateValue = entry.Value;
                    }
                }
                double sample = reward + _gamma * maxNextStateValue;
                //Console.WriteLine(@"r {0} {1} {2}", sample, reward, gamma * maxNextStateValue);
                double prevQValue = _qValues[prevState][action];
                _qValues[prevState][action] = (1 - _alpha) * prevQValue + _alpha * sample;
            }
            else
            {
                double sample = reward + _gamma * 0;
                //Console.WriteLine(@"r {0} {1} {2}", sample, reward, gamma * maxNextStateValue);
                double prevQValue = _qValues[prevState][action];
                _qValues[prevState][action] = (1 - _alpha) * prevQValue + _alpha * sample;
            }

            //Console.WriteLine(" ------ ");
            //QValDisp();
        }

        //dynamicaly create Pair state-state if not already created
        public void CreateAction(State prevState, State nextState)
        {
            Dictionary<State, double> statesFromAState;

            if (_qValues.TryGetValue(prevState, out statesFromAState))
            {
                double tempReward;
                if (!statesFromAState.TryGetValue(nextState, out tempReward))
                {
                    statesFromAState.Add(nextState, 0);
                }
            }
            else
            {
                statesFromAState = new Dictionary<State, double> { { nextState, 0 } };
                _qValues.Add(prevState, statesFromAState);
            }
        }

        //get action with max q_value
        //!!!!! if only negative q_values it still chooses from them
        public State GetNextOptimalState(State state, List<State> relevantStates)
        {
            Dictionary<State, double> statesFromAState;
            _qValues.TryGetValue(state, out statesFromAState);
            double maxValue = double.MinValue;
            State maxState = state;

            if (statesFromAState == null) Console.Write(@"Something is ***** wrong");

            foreach (State s in relevantStates)
            {
                double tempValue;
                if (statesFromAState != null && statesFromAState.TryGetValue(s, out tempValue))
                {
                    if (maxValue <= tempValue)
                    {
                        maxState = s;
                        maxValue = tempValue;
                    }
                }
            }
            //if (maxValue > 0) Console.WriteLine(maxValue);
            return maxState;
        }

        //get random q_state
        public State GetNextRandomState(State state, List<State> relevantStates)
        {
            Random rnd = new Random();
            int randomIndex = rnd.Next(relevantStates.Count);
            State nextState = relevantStates[randomIndex];
            CreateAction(state, nextState);

            return nextState;
        }

        //get the next action by the rules of q_learning (alpha,gamma,eps)
        public State GetNextState(State state, List<State> relevantStates)
        {
            if (relevantStates == null) Console.Write(@"Something is ***** wrong");

            Dictionary<State, double> statesFromAState;
            Random rnd = new Random();
            double randomDouble = rnd.NextDouble();

            if (!_qValues.TryGetValue(state, out statesFromAState)) return GetNextRandomState(state, relevantStates);
            if (randomDouble <= _epsilon) return GetNextRandomState(state, relevantStates);
            return GetNextOptimalState(state, relevantStates);
        }

        /*public void saveQ_valuesToFile(string fileName)
        {
            string s = "";

            foreach (var key in _qValues.Keys)
            {
                s += key.IntState + " ";
                foreach (var innerKey in _qValues[key].Keys)
                {
                    //Console.WriteLine(_qValues[key].Keys.Count);
                    //Console.Write(@" {0} v:{1}", innerKey, _qValues[key][innerKey]);
                    s += innerKey.IntState + ":" + _qValues[key][innerKey] + " ";
                }
                s += Environment.NewLine;
            }

            File.WriteAllText(fileName, s);
        }

        public void readQ_valuesFromFile(StreamReader sr)
        {
            // string readText = File.ReadAllText("q_values.txt");

            char[] delimiterChars1 = { ' ' };
            char[] delimiterChars2 = { ':' };
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                var states = new Dictionary<State, double>();
                var data = line.Split(delimiterChars1);
                var state1 = new State(short.Parse(data[0]));
                for (int i = 1; i < data.Length; i++)
                {
                    var data2 = data[i].Split(delimiterChars2);
                    var state = new State(short.Parse(data2[0]));
                    var qValue = double.Parse(data2[1]);
                    states.Add(state, qValue);
                }
                _qValues.Add(state1, states);
            }
        }*/

        public int StatCount()
        {
            return _qValues.Keys.Count;
        }
    }
}
