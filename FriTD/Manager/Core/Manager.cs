using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using Manager.AI;
using Manager.AIUtils;
using Manager.Core.Delayers;
using Manager.GameStates;
using Manager.Kohonen;
using Manager.QLearning;
using TD.Core;
using TD.Enums;


namespace Manager.Core
{
    public class Manager
    {
        private TDGame _game;
        private IAiAdapter _aiAdapter;
        private AICore _ai;
        private KohonenCore<StateVector> _kohonen;
        private int _iteration;

        private QLearning<KohonenAiState> _qLearning;


        private int firstGameWonLevel;
        private int won;
        private int numOfStates;


        //private DataStore _store;
        public DataStore _store { get; set; }
        private readonly IDelayer _delayer;

        public Manager()
        {
            _store = new DataStore();
            //_delayer = new SimpleDelayer();
            _delayer = new LearningDelayer();
        }

        public void StatisticsRun()
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine("run num: " + i);
                firstGameWonLevel = -1;
                won = 0;

                this.InsertAi();
                AiLearningRun();
                numOfStates = _ai.StatCount();
                Statistics.counter++;
                Statistics.endgameStatesNum += numOfStates;
                if (firstGameWonLevel > 0)
                {
                    Statistics.firstWonRound += firstGameWonLevel;
                    Statistics.gamesWithVictory++;
                }

                Statistics.gamesWon += won;
                Statistics.gamesLost += 1000 - won;
            }

            Console.WriteLine("count: {0} states: {1} first game won: {2} won: {3} lost: {4}", Statistics.counter,
                Statistics.endgameStatesNum*1.0/Statistics.counter,
                Statistics.firstWonRound*1.0/Statistics.gamesWithVictory, Statistics.gamesWon*1.0/Statistics.counter,
                Statistics.gamesLost*1.0/Statistics.counter);
        }

        internal void InsertAi(StreamReader reader)
        {
            _ai = new AICore(0.1, 1, 0.5, reader);
            _aiAdapter = new GameStateManager(_ai);
        }

        public bool IsAiMode()
        {
            return _ai != null;
        }

        public void PrepareGame()
        {
            _game = new TDGame();
            _game.InitGame(Properties.Resources.Towers, Properties.Resources.Map, Properties.Resources.Enemies,
                Properties.Resources.Levels);
        }

        public void StartTurn()
        {
            _game.StartLevel();
            ExecuteLevel();
        }


        public void InsertAi()
        {
            _ai = new AICore(0.1, 1, 0.5);
            // _aiAdapter = new GameStateManager(_ai);
            Console.WriteLine("AI inserted");
            _qLearning = new QLearning<KohonenAiState>(0.3, 1, 0.5);
            // _aiAdapter = new NewGameStateManager(_qLearning);
            _kohonen = new KohonenCore<StateVector>(30, 30, 2, 0.5, 1, 1, 0.5, false);
            //_aiAdapter = new KohonenGameStateManager(_qLearning,_kohonen);
            _aiAdapter = new KohonenGameStateManagerSemiInteligentActions(_qLearning, _kohonen);
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

        // ----------- unity hack end -------------


        private void ExecuteLevel()
        {
            while (_game.State == GameState.InProgress)
            {
                _game.Tic();
                _store.ExchangeData(_game.GameVisualImage());
                _delayer.Delay();
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

        public void StartAiDrivenTurn()
        {
            int limit = 500;

            var decisionResult = _aiAdapter.ExecuteDecision(_game.GameStateImage());
            if (_iteration > limit)
            {
                Console.WriteLine(decisionResult);
                Console.WriteLine("from");
                GameStateImage img = _game.GameStateImage();
                Console.WriteLine(img.Gold);
                foreach (int tower in img.Towers)
                {
                    Console.Write(tower);
                }
                Console.WriteLine();
                if (img.Level > 3) Console.WriteLine(img.Level);
            }
            var arr = decisionResult.Split(' ');

            foreach (var cmd in arr)
            {
                
                if (cmd.Length > 0) ExecuteCmd(cmd);
            }

            if (_iteration > limit)
            {
                
                Console.WriteLine("to");
                GameStateImage img = _game.GameStateImage();
                foreach (int tower in img.Towers)
                {
                    Console.Write(tower);
                }
                Console.WriteLine();
                if (img.Level > 3) Console.WriteLine(img.Level);
            }

            StartTurn();

            _aiAdapter.ExecuteReward(_game.GameStateImage());
        }

        public void AiLearningRun()
        {
            int won = 0;
            int lost = 0;
            Console.WriteLine(DateTime.Now.ToString());
            int innerInterval = 100;
            int iterations = 40;
            //_kohonen.Displ();
            ((KohonenGameStateManagerSemiInteligentActions) _aiAdapter).SetRewardMultiplier(1.0/10000);
            for (int i = 0; i < iterations; i++)
            {
                _iteration = i;
                //TODO remove
                if (i == 150)
                {
                    ((KohonenGameStateManagerSemiInteligentActions) _aiAdapter).disableLearning();
                    ((KohonenGameStateManagerSemiInteligentActions) _aiAdapter).SetRewardMultiplier(1);
                }


                for (int j = 0; j < innerInterval; j++)
                {
                    if (GameState.Won == SingleAiLongRunIteration())
                    {
                        if (firstGameWonLevel == -1) firstGameWonLevel = i*innerInterval + j;
                        won++;
                    }
                    else
                    {
                        lost++;
                    }

                    {
                      /*  if (i > 2)
                        {
                            int counter = 0;
                            GameStateImage img = _game.GameStateImage();
                            foreach (int tower in img.Towers)
                            {
                                if (tower > -1) counter++;
                            }

                            if (counter > 2)
                            {
                                Console.WriteLine(counter);
                                foreach (int tower in img.Towers)
                                {
                                    Console.Write(tower+" ");
                                }
                                Console.WriteLine();
                            }
                            if (img.Level > 3) Console.WriteLine(img.Level);
                        }*/
                        
                    }
                }
                // _ai.saveQ_valuesToFile(@"C:\Users\Tomas\Desktop\copy\"+i);
                this.won += won;
                Console.WriteLine("Iteration: " + i + " won: " + won + " lost: " + lost);
                won = 0;
                lost = 0;
            }
            Console.WriteLine(DateTime.Now.ToString());
            Console.WriteLine("--------------------------------------------------------------");
            // _kohonen.Displ();

            // _ai.QValDisp();
            _qLearning.QValDisp();
        }

        private GameState SingleAiLongRunIteration()
        {
            PrepareGame();
            // _game.BuildTower(3,1);
            while (_game.State == GameState.Waiting)
            {
                StartAiDrivenTurn();
            }
            // _ai.QValDisp();

            //TODO remove

            return _game.State;
        }
    }
}
