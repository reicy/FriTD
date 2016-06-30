using System;
using System.Collections.Concurrent;
using System.Threading;
using Manager.GameStates;
using Manager.Kohonen;
using Manager.QLearning;
using TD.Core;
using TD.Enums;

namespace Manager.MTCore
{
    public class MtSingleDaemon
    {
        public KohonenCore<StateVector> Kohonen { get; }
        public QLearning<KohonenAiState> QLearning { get; }
        public BlockingCollection<KohonenUpdate> UpdatesQueue { get; }
        public int Lost { get; set; }
        public int Won { get; set; }
        public int IterationStartLearning { get; set; }
        private TDGame _game;
        private MtAiAdapter _aiAdapter;

  
        public MtSingleDaemon(KohonenCore<StateVector> kohonen, QLearning<KohonenAiState> qLearning,
            BlockingCollection<KohonenUpdate> updatesQueue)
        {
            Kohonen = kohonen;
            QLearning = qLearning;
            UpdatesQueue = updatesQueue;
            Won = 0;
            Lost = 0;
            
        }

        public void ProcessLearning()
        {
            _aiAdapter = new MtAiAdapter(QLearning, Kohonen);
            _aiAdapter.SetRewardMultiplier(1.0/10000);
            KohonenUpdate update;
            int iteration = 0;
            
            while (true)
            {
                
                iteration++;
                if (iteration%IterationStartLearning == 0)
                {
                    _aiAdapter.SetRewardMultiplier(1);
                }

                if (GameState.Won == RunIteration())
                {
                    Won++;
                    MtStats.IncWL(1);
                }
                else
                {
                    Lost++;
                    MtStats.IncWL(0);
                }
                

                update = _aiAdapter.KohonenUpdate;
                
                UpdatesQueue.Add(update);
                



            }
        }


        private GameState RunIteration()
        {
            PrepareGame();

            while (_game.State == GameState.Waiting)
            {
                StartAiDrivenTurn();
            }

           

            return _game.State;
        }


        public void PrepareGame()
        {
            _game = new TDGame();
            _game.InitGame(Properties.Resources.Towers, Properties.Resources.Map, Properties.Resources.Enemies,
                Properties.Resources.Levels);
        }

        public void StartAiDrivenTurn()
        {
            //ask ai for commands
            var decisionResult = _aiAdapter.ExecuteDecision(_game.GameStateImage());

            //process ai commands
            var arr = decisionResult.Split(' ');
            foreach (var cmd in arr)
            {
                if (cmd.Length > 0) ExecuteCmd(cmd);
            }

            //start level
            _game.StartLevel();
            ExecuteLevel();

            //reward ai
            _aiAdapter.ExecuteReward(_game.GameStateImage());
        }

        private void ExecuteLevel()
        {
            while (_game.State == GameState.InProgress)
            {
                _game.Tic();
            }
        }

        public void ExecuteCmd(string cmd)
        {
            if (cmd[0] == 'b')
            {
                var arr = cmd.Split('_');
                _game.BuildTower(int.Parse(arr[1]), int.Parse(arr[2]));
            }
            if (cmd[0] == 's')
            {
                var arr = cmd.Split('_');
                _game.SellTower(int.Parse(arr[1]));
            }
        }
    }
}