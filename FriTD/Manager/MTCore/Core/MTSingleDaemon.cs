using System;
//using System.Collections.Concurrent;
using System.Threading;
using Manager.GameStates;
using Manager.Kohonen;
using Manager.MTCore.KohonenUtils;
using Manager.QLearning;
using TD.Core;
using TD.Enums;
using Manager.Core;
using System.Collections.Generic;

namespace Manager.MTCore
{
    public class MtSingleDaemon
    {
        public KohonenCore<StateVector> Kohonen { get; }
        public QLearning<KohonenAiState> QLearning { get; }
        public List<KohonenUpdate> UpdatesQueue { get; }
        public int Lost { get; set; }
        public int Won { get; set; }
        public int IterationStartLearning { get; set; }
        private TDGame _game;
        private string _map;
        private MtAiAdapter _aiAdapter;
        private string _levels;
        private int _type;
        private bool _heuristicActive;
        private bool _cosinusDistActive;
        public DataStore _store { get; }

        public MtSingleDaemon(KohonenCore<StateVector> kohonen, QLearning<KohonenAiState> qLearning,
            List<KohonenUpdate> updatesQueue, string map)
        {
            _store = new DataStore();
            Kohonen = kohonen;
            QLearning = qLearning;
            UpdatesQueue = updatesQueue;
            Won = 0;
            Lost = 0;
            _map = map;


        }

        public MtSingleDaemon(KohonenCore<StateVector> kohonen, QLearning<KohonenAiState> qLearning, List<KohonenUpdate> updatesQueue, string map, string levels1, int type, bool heuristicActive, bool cosinusDistActive) : this(kohonen, qLearning, updatesQueue, map)
        {
            this._levels = levels1;
            this._type = type;
            _heuristicActive = heuristicActive;
            _cosinusDistActive = cosinusDistActive;

        }

        public void ProcessLearning()
        {
            //_aiAdapter = new MtAiAdapter(QLearning, Kohonen, new SimpleStateEncodern());
            _aiAdapter = new MtAiAdapter(QLearning, Kohonen, new AdaptiveStateEncoder(), _heuristicActive, _cosinusDistActive);
            _aiAdapter.SetRewardMultiplier(1.0/10000);
            KohonenUpdate update;
            int iteration = 0;
            
            while (true)
            {
                
                iteration++;
                if (iteration==IterationStartLearning)
                {
                    _aiAdapter.SetRewardMultiplier(1);
                }

                if (GameState.Won == RunIteration())
                {
                    Won++;
                    MtStats.IncWL(1, _game.GameStateImage().Level, _type);
                }
                else
                {
                    Lost++;
                    MtStats.IncWL(0, _game.GameStateImage().Level, _type);
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
            _game.InitGame(Properties.Resources.Towers, _map, Properties.Resources.Enemies,
                _levels);
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

        // ----------- unity hack begin ------------
        public void UnityStartLevel()
        {
            if (_game.State == GameState.Won || _game.State == GameState.Lost)
            {
                PrepareGame();
            }

            if (_game.State != GameState.InProgress)
            {
                if (IsAiMode())
                {
                    var decisionResult = _aiAdapter.ExecuteDecision(_game.GameStateImage());
                    var arr = decisionResult.Split(' ');
                    foreach (var cmd in arr)
                    {
                        if (cmd.Length > 0) ExecuteCmd(cmd);
                    }

                    _game.StartLevel();
                }
                else
                {
                    _game.StartLevel();
                }
                _store.ExchangeData(_game.GameVisualImage());
            }
        }

        public void UnityTic()
        {
            if (_game.State == GameState.InProgress)
            {
                _game.Tic();
                _store.ExchangeData(_game.GameVisualImage());

                if (_game.State != GameState.InProgress && IsAiMode())
                {
                    _aiAdapter.ExecuteReward(_game.GameStateImage());
                }
            }
        }

        public GameState GetGameState()
        {
            return _game.State;
        }

        // TODO !!!
        private bool IsAiMode()
        {
            return false;
        }

        // ----------- unity hack end -------------
    }
}