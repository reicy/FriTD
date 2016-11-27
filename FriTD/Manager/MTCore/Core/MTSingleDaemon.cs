using System.Collections.Concurrent;
using Manager.Kohonen;
using Manager.MTCore.Adapters;
using Manager.MTCore.Core;
using Manager.MTCore.KohonenUtils;
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
        private string _map;
        private int _mapNumber;
        private MtAiAdapter _aiAdapter;
        private string _levels;
        private int _type;
        private bool _heuristicActive;
        private bool _cosinusDistActive;

        public MtSingleDaemon(KohonenCore<StateVector> kohonen, QLearning<KohonenAiState> qLearning,
            BlockingCollection<KohonenUpdate> updatesQueue, string map)
        {
            Kohonen = kohonen;
            QLearning = qLearning;
            UpdatesQueue = updatesQueue;
            Won = 0;
            Lost = 0;
            _map = map;


        }

        public MtSingleDaemon(KohonenCore<StateVector> kohonen, QLearning<KohonenAiState> qLearning, BlockingCollection<KohonenUpdate> updatesQueue, string map, string levels1, int type, bool heuristicActive, bool cosinusDistActive, int mapNumber = 0) : this(kohonen, qLearning, updatesQueue, map)
        {
            _levels = levels1;
            _type = type;
            _heuristicActive = heuristicActive;
            _cosinusDistActive = cosinusDistActive;
            _mapNumber = mapNumber;
        }

        public void ProcessLearning()
        {
            //_aiAdapter = new MtAiAdapter(QLearning, Kohonen, new SimpleStateEncodern());
            _aiAdapter = new MtAiAdapter(QLearning, Kohonen, new AdaptiveStateEncoder(), _heuristicActive, _cosinusDistActive);
            _aiAdapter.SetRewardMultiplier(1.0/10000);
            KohonenUpdate update;
            var iteration = 0;
            
            while (true)
            {
                iteration++;
                if (iteration==IterationStartLearning)
                    _aiAdapter.SetRewardMultiplier(1);

                if (GameState.Won == RunIteration())
                {
                    Won++;
                    MtStats.IncWL(1, _game.GameStateImage().Level, _type, _mapNumber);
                }
                else
                {
                    Lost++;
                    MtStats.IncWL(0, _game.GameStateImage().Level, _type, _mapNumber);
                }

                update = _aiAdapter.KohonenUpdate;
                
                UpdatesQueue.Add(update);
            }
        }


        private GameState RunIteration()
        {
            PrepareGame();

            while (_game.State == GameState.Waiting)
                StartAiDrivenTurn();
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
                if (cmd.Length > 0) ExecuteCmd(cmd);

            //start level
            _game.StartLevel();
            ExecuteLevel();

            //reward ai
            _aiAdapter.ExecuteReward(_game.GameStateImage());
        }

        private void ExecuteLevel()
        {
            while (_game.State == GameState.InProgress)
                _game.Tic();
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