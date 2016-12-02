using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace Manager.QLearning
{
    [Synchronization]
    //class QLearning<State> where State : IVector<State>/*, new()*/
    public class QLearning<TState, TAction> where TState : QState, new() where TAction : class, QAction, new()
    {
        public double Epsilon { get; set; }
        public double Gamma { get; set; }
        public double Alpha { get; set; }
        //int iterationCounter = 0;
        //int iterationsToSave = 100;
        readonly Dictionary<TState, Dictionary<TAction, double>> _qValues;
        readonly Random _explorationGen = new Random();

        /// <param name="epsilon">pravdebodobnost, ci vykonam random akciu</param>
        /// <param name="gamma">discount factor</param>
        /// <param name="alpha">learning rate</param>
        public QLearning(double epsilon, double gamma, double alpha)
        {
            Alpha = alpha;
            Epsilon = epsilon;
            Gamma = gamma;
            _qValues = new Dictionary<TState, Dictionary<TAction, double>>();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void updateQ_values(TState prevState, TAction action, TState nextState, double reward)
        {
            //Console.WriteLine(reward);
            double maxNextActionValue = double.MinValue;

            // createAction(state, action); //?????? needed?

            Dictionary<TAction, double> actionsFromNextState;// = _qValues[nextState];
            if (_qValues.TryGetValue(nextState, out actionsFromNextState))
            {
                foreach (KeyValuePair<TAction, double> entry in actionsFromNextState)
                {
                    if (entry.Value >= maxNextActionValue)
                    {
                        maxNextActionValue = entry.Value;
                    }
                }
                double sample = reward + Gamma * maxNextActionValue;
                //Console.WriteLine(@"r {0} {1} {2}", sample, reward, _gamma * maxNextStateValue);
                double prevQValue = _qValues[prevState][action];
                _qValues[prevState][action] = (1 - Alpha) * prevQValue + Alpha * sample;
            }
            else
            {
                double sample = reward + Gamma * 0;
                //Console.WriteLine(@"r {0} {1} {2}", sample, reward, _gamma * maxNextStateValue);
                double prevQValue = _qValues[prevState][action];
                _qValues[prevState][action] = (1 - Alpha) * prevQValue + Alpha * sample;
            }
        }

        //dynamicaly create Pair state-state if not already created
        //if already in q tree nothing happens
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CreateAction(TState state, TAction newAction)
        {
            Dictionary<TAction, double> actionsFromAState;

            if (_qValues.TryGetValue(state, out actionsFromAState))
            {
                double tempReward;
                if (!actionsFromAState.TryGetValue(newAction, out tempReward))
                {
                    actionsFromAState.Add(newAction, 0);
                }
            }
            else
            {
                actionsFromAState = new Dictionary<TAction, double> { { newAction, 0 } };
                _qValues.Add(state, actionsFromAState);
            }
        }

        //get action with max q_value
        //!!!!! if only negative q_values it still chooses from them
        [MethodImpl(MethodImplOptions.Synchronized)]
        public TAction GetNextOptimalAction(TState state, List<TAction> possibleActions)
        {
            Dictionary<TAction, double> actionsFromAState;
            _qValues.TryGetValue(state, out actionsFromAState);
            double maxValue = double.MinValue;
            TAction bestAction = null;

            if (actionsFromAState == null) Console.Write(@"Something is ***** wrong in getNextOptimalAction1");

            foreach (var action in possibleActions)
            {
                double tempValue;
                if (actionsFromAState != null && actionsFromAState.TryGetValue(action, out tempValue))
                {
                    if (maxValue <= tempValue)
                    {
                        bestAction = action;
                        maxValue = tempValue;
                    }
                }
            }
            //if (maxValue > 0) Console.WriteLine(maxValue);
            //if (bestAction == null) Console.Write(@"Something is ***** wrong in getNextOptimalAction2");
            return bestAction;
        }

        //get random q_state
        [MethodImpl(MethodImplOptions.Synchronized)]
        public TAction GetNextRandomAction(TState state, List<TAction> possibleActions)
        {
            if (possibleActions.Count == 0) Console.Write(@"Something is ***** wrong in getNextRandomAction-- no possible actions");
            int randomIndex = _explorationGen.Next(possibleActions.Count);
            TAction nextAction = possibleActions[randomIndex];
            CreateAction(state, nextAction);

            return nextAction;
        }

        //get the next action by the rules of q_learning (alpha,gamma,eps)
        [MethodImpl(MethodImplOptions.Synchronized)]
        public TAction GetNextAction(TState state, List<TAction> possibleActions)
        {
            if (possibleActions == null) Console.Write(@"Something is ***** wrong in getNextAction --- no possible actions");

            Dictionary<TAction, double> actionsFromAState;
            double randomDouble = _explorationGen.NextDouble();

            if (!_qValues.TryGetValue(state, out actionsFromAState)) return GetNextRandomAction(state, possibleActions);
            if (randomDouble <= Epsilon) return GetNextRandomAction(state, possibleActions);

            TAction optAction = GetNextOptimalAction(state, possibleActions);
            if (optAction == null) return GetNextRandomAction(state, possibleActions);
            return optAction;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
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
                    Console.Write(@"({0}    {1})", innerKey, _qValues[key][innerKey]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            //Console.WriteLine();
        }

        /// <param name="percentage">Percentage in interval [0.0, 1.0]</param>
        public void DecreaseLearningRateBy(double percentage)
        {
            Alpha -= percentage * Alpha;
        }

        /// <param name="percentage">Percentage in interval [0.0, 1.0]</param>
        public void DecreaseEpsilonBy(double percentage)
        {
            Epsilon -= percentage * Epsilon;
        }

        public void Save(string filename)
        {
            var sb = new StringBuilder();

            sb.Append($"{_qValues.Keys.Count}{Environment.NewLine}");
            foreach (var key in _qValues.Keys)
            {
                sb.Append($"{key} - ");
                foreach (var innerKey in _qValues[key].Keys)
                {
                    sb.Append($"({innerKey}    {_qValues[key][innerKey]})");
                }
                sb.Append(Environment.NewLine);
            }
            sb.Append(Environment.NewLine);

            File.WriteAllText(filename, sb.ToString());
        }

        public void Load(string filename)
        {
            _qValues.Clear();

            using (var reader = new StreamReader(new FileStream(filename, FileMode.Open)))
            {
                reader.ReadLine();
                while (true)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        break;
                    line = line.Trim();
                    var data = line.Split(new[] {" - "}, StringSplitOptions.RemoveEmptyEntries);
                    var state = new TState();
                    state.FromString(data[0]);
                    var actionValueDict = new Dictionary<TAction, double>();
                    var actionValues = data[1].Split(new[] {'(', ')'}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var actionValue in actionValues)
                    {
                        var tmp = actionValue.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                        var action = new TAction();
                        action.FromString(tmp[0]);
                        var d = double.Parse(tmp[1]);
                        actionValueDict.Add(action, d);
                    }
                    _qValues.Add(state, actionValueDict);
                }
            }

            Console.WriteLine(@"Successfully loaded QLearning from '{0}'", filename);
        }

        //update q_Value of a pair state-state(action) after executing and getting the reward
        /*public void UpdateQValues1(State prevState, State nextState, State action, double reward)
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
                //Console.WriteLine(@"r {0} {1} {2}", sample, reward, _gamma * maxNextStateValue);
                double prevQValue = _qValues[prevState][nextState];
                _qValues[prevState][nextState] = (1 - _alpha) * prevQValue + _alpha * sample;
            }

            //Console.WriteLine(@" ------ ");
            //QValDisp();
        }*/

        //dynamicaly create Pair state-state if not already created  ----OLD
        /*public void CreateAction(State prevState, State nextState)
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
                statesFromAState = new Dictionary<State, double> {{nextState, 0}};
                _qValues.Add(prevState, statesFromAState);
            }
        }*/

        /*public void SaveQValuesToFile(string fileName)
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

        public void ReadQValuesFromFile(StreamReader sr)
        {
            //string readText = File.ReadAllText("q_values.txt");

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

        /*public int StatCount()
        {
            return _qValues.Keys.Count;
        }*/
    }
}
