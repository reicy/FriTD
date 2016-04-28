using Manager.Kohonen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manager.QLearning
{
    //class QLearning<State> where State : IVector<State>/*, new()*/
    public class QLearning<TState> where TState : QState/*, new()*/
    {
        double epsilon, gamma, alpha;
        int iterationCounter = 0;
        //int iterationsToSave = 100;
        Dictionary<TState, Dictionary<QAction, double>> q_values;
        Random explorationGen = new Random();

        public QLearning(double e, double g, double a)
        {
            alpha = a;
            epsilon = e;
            gamma = g;
            q_values = new Dictionary<TState, Dictionary<QAction, double>>();
        }


         public void updateQ_values(TState prevState, QAction action,TState nextState , double reward)
         {
             //Console.WriteLine(reward);
             double maxNextActionValue = double.MinValue;
             QAction maxNextAction;

            // createAction(state, action); //?????? needed?

             Dictionary<QAction, double> actionsFromNextState;// = q_values[nextState];
             if (q_values.TryGetValue(nextState, out actionsFromNextState))
             {

                 foreach (KeyValuePair<QAction, double> entry in actionsFromNextState)
                 {
                     if (entry.Value >= maxNextActionValue)
                     {
                         maxNextActionValue = entry.Value;
                         maxNextAction = entry.Key;
                     }
                 }
                 double sample = reward + gamma * maxNextActionValue;
                 //  Console.WriteLine("r "+sample+ " "+reward +" "+gamma*maxNextStateValue);
                 double prevQ_value = q_values[prevState][action];
                 q_values[prevState][action] = (1 - alpha) * prevQ_value + alpha * sample;
             }
             else
             {
                 double sample = reward + gamma * 0;
                 //  Console.WriteLine("r "+sample+ " "+reward +" "+gamma*maxNextStateValue);
                 double prevQ_value = q_values[prevState][action];
                 q_values[prevState][action] = (1 - alpha) * prevQ_value + alpha * sample;
             }
         }


        //dynamicaly create Pair state-state if not already created
        //if already in q tree nothing happens
        public void createAction(TState state, QAction newAction)
        {
            Dictionary<QAction, double> actionsFromAState;
            double tempReward;

            if (q_values.TryGetValue(state, out actionsFromAState))
            {
                if (!actionsFromAState.TryGetValue(newAction, out tempReward))
                {
                    actionsFromAState.Add(newAction, 0);
                }
            }
            else
            {
                actionsFromAState = new Dictionary<QAction, double>();
                actionsFromAState.Add(newAction, 0);
                q_values.Add(state, actionsFromAState);
            }
        }

        //get action with max q_value
        //!!!!! if only negative q_values it still chooses from them
        public QAction getNextOptimalAction(TState state, List<QAction> possibleActions)
        {
            Dictionary<QAction, double> actionsFromAState;
            q_values.TryGetValue(state, out actionsFromAState);
            double maxValue = double.MinValue;
            QAction bestAction = null;
            double tempValue;

            if (actionsFromAState == null) Console.Write("Something is ***** wrong in getNextOptimalAction1");

            foreach (QAction A in possibleActions)
            {
                if (actionsFromAState.TryGetValue(A, out tempValue))
                {
                    if (maxValue <= tempValue)
                    {
                        bestAction = A;
                        maxValue = tempValue;
                    }


                }
            }
           // if(maxValue > 0) Console.WriteLine(maxValue);
            if (bestAction == null) Console.Write("Something is ***** wrong in getNextOptimalAction2");
            return bestAction;
        }

        //get random q_state
         public QAction getNextRandomAction(TState state, List<QAction> possibleActions)
         {
             if (possibleActions.Count == 0) Console.Write("Something is ***** wrong in getNextRandomAction-- no possible actions");
             int randomIndex = explorationGen.Next(possibleActions.Count);
             QAction nextAction = possibleActions[randomIndex];
             createAction(state, nextAction);

             return nextAction;
         }

        //get the next action by the rules of q_learning (alpha,gamma,eps)
         public QAction getNextAction(TState state, List<QAction> possibleActions)
         {
             if (possibleActions == null) Console.Write("Something is ***** wrong in getNextAction --- no possible actions");

             Dictionary<QAction, double> actionsFromAState;
             double randomDouble = explorationGen.NextDouble();

             if (!q_values.TryGetValue(state, out actionsFromAState)) return getNextRandomAction(state, possibleActions);
             else if (randomDouble <= epsilon) return getNextRandomAction(state, possibleActions);
             else
             {
                 QAction optAction = getNextOptimalAction(state, possibleActions);
                 if (optAction == null) return getNextRandomAction(state, possibleActions);
                 return optAction;
             }
         }




        public void QValDisp()
        {
            //Console.WriteLine("hej");
            Console.WriteLine(q_values.Keys.Count);
            foreach (var key in q_values.Keys)
            {
                Console.Write(key.ToString() + " - ");
                foreach (var innerKey in q_values[key].Keys)
                {
                    //Console.WriteLine(q_values[key].Keys.Count);
                    // Console.Write(" {0} v:{1}",innerKey.toString(),(q_values[key])[innerKey]);
                    Console.Write(innerKey.ToString() + "    " + q_values[key][innerKey]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            //Console.WriteLine();
        }

        //update q_Value of a pair state-state(action) after executing and getting the reward
        /*public void updateQ_values1(State prevState, State nextState, State action, double reward)
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
                double sample = reward + gamma * maxNextStateValue;
                //  Console.WriteLine("r "+sample+ " "+reward +" "+gamma*maxNextStateValue);
                double prevQ_value = q_values[prevState][nextState];
                q_values[prevState][nextState] = (1 - alpha) * prevQ_value + alpha * sample;
            }




            //     Console.WriteLine(" ------ ");
            //     QValDisp();

        }*/


        //dynamicaly create Pair state-state if not already created  ----OLD
        /* public void createAction(State prevState, State nextState)
         {
             Dictionary<State, double> StatesFromAState;
             double tempReward;

             if (q_values.TryGetValue(prevState, out StatesFromAState))
             {
                 if (!StatesFromAState.TryGetValue(nextState, out tempReward))
                 {
                     StatesFromAState.Add(nextState, 0);
                 }
             }
             else
             {
                 StatesFromAState = new Dictionary<State, double>();
                 StatesFromAState.Add(nextState, 0);
                 q_values.Add(prevState, StatesFromAState);
             }
         }*/


        /*    public void saveQ_valuesToFile(string fileName)
            {
                string s = "";

                foreach (var key in q_values.Keys)
                {
                    s+=key.IntState + " ";
                    foreach (var innerKey in q_values[key].Keys)
                    {
                        //Console.WriteLine(q_values[key].Keys.Count);
                        // Console.Write(" {0} v:{1}",innerKey.toString(),(q_values[key])[innerKey]);
                        s+=innerKey.IntState + ":" + q_values[key][innerKey]+" ";
                    }
                    s += Environment.NewLine;
                }

                File.WriteAllText(fileName, s);
            }

            public void readQ_valuesFromFile(StreamReader sr)
            {
               // string readText = File.ReadAllText("q_values.txt");

                char[] delimiterChars1 = {' '};
                char[] delimiterChars2 = { ':' };
                string line;
                string[] data;
                string[] data2;
                State state;
                State state1;
                double q_value;
                Dictionary<State, double> states;

                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    states = new Dictionary<State, double>();
                    data = line.Split(delimiterChars1);
                    state1= new State(Int16.Parse(data[0]));
                    for (int i = 1; i < data.Length; i++)
                    {
                        data2 = data[i].Split(delimiterChars2);
                        state = new State(Int16.Parse(data2[0]));
                        q_value = Double.Parse(data2[1]);
                        states.Add(state, q_value);
                    }
                    q_values.Add(state1, states);
                }


            }


        */

        /*  public int StatCount()
          {
              return q_values.Keys.Count;
          }*/
    }
}
