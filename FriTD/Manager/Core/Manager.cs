using System;
using System.Collections.Generic;
using System.IO;
using Manager.AI;
using Manager.AIUtils;
using Manager.Delayers;
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
        private AiCore _ai;
        private KohonenCore<StateVector> _kohonen;
        private int _iteration;

        private QLearning<KohonenAiState> _qLearning;

        private int _firstGameWonLevel;
        private int _won;
        private int _numOfStates;

        private readonly DataStore _store;
        //public DataStore _store { get; set; }
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
                Console.WriteLine(@"run num: {0}", i);
                _firstGameWonLevel = -1;
                _won = 0;

                InsertAi();
                AiLearningRun();
                _numOfStates = _ai.StatCount();
                Statistics.Counter++;
                Statistics.EndgameStatesNum += _numOfStates;
                if (_firstGameWonLevel > 0)
                {
                    Statistics.FirstWonRound += _firstGameWonLevel;
                    Statistics.GamesWithVictory++;
                }

                Statistics.GamesWon += _won;
                Statistics.GamesLost += 1000 - _won;
            }

            Console.WriteLine(@"count: {0} states: {1} first game won: {2} won: {3} lost: {4}", Statistics.Counter,
                Statistics.EndgameStatesNum * 1.0 / Statistics.Counter,
                Statistics.FirstWonRound * 1.0 / Statistics.GamesWithVictory, Statistics.GamesWon * 1.0 / Statistics.Counter,
                Statistics.GamesLost * 1.0 / Statistics.Counter);
        }

        internal void InsertAi(StreamReader reader)
        {
            _ai = new AiCore(0.1, 1, 0.5, reader);
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

        public void PrepareGame(string mapFile, string levelsFile)
        {
            _game = new TDGame();
            _game.InitGame(Properties.Resources.Towers, mapFile, Properties.Resources.Enemies, levelsFile);
        }

        public void StartTurn()
        {
            _game.StartLevel();
            ExecuteLevel();
        }

        public void InsertAi()
        {
            _ai = new AiCore(0.1, 1, 0.5);
            // _aiAdapter = new GameStateManager(_ai);
            Console.WriteLine(@"AI inserted");
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
                Console.WriteLine(@"from");
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

                Console.WriteLine(@"to");
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
            var won = 0;
            var lost = 0;
            Console.WriteLine(DateTime.Now);
            const int innerInterval = 100;
            const int iterations = 40;
            //_kohonen.Displ();
            var aiAdapter = (KohonenGameStateManagerSemiInteligentActions)_aiAdapter;
            aiAdapter.SetRewardMultiplier(1.0 / 10000);
            for (int i = 0; i < iterations; i++)
            {
                _iteration = i;
                //TODO remove
                if (i == 150)
                {
                    aiAdapter.DisableLearning();
                    aiAdapter.SetRewardMultiplier(1);
                }


                for (int j = 0; j < innerInterval; j++)
                {
                    if (GameState.Won == SingleAiLongRunIteration())
                    {
                        if (_firstGameWonLevel == -1) _firstGameWonLevel = i * innerInterval + j;
                        won++;
                    }
                    else
                    {
                        lost++;
                    }

                    {
                        /*if (i > 2)
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
                                    Console.Write(@"{0} ", tower);
                                }
                                Console.WriteLine();
                            }
                            if (img.Level > 3) Console.WriteLine(img.Level);
                        }*/
                    }
                }
                //_ai.saveQ_valuesToFile(@"C:\Users\Tomas\Desktop\copy\" + i);
                _won += won;
                Console.WriteLine(@"Iteration: {0} won: {1} lost: {2}", i, won, lost);
                won = 0;
                lost = 0;
            }
            Console.WriteLine(DateTime.Now);
            Console.WriteLine(@"--------------------------------------------------------------");
            // _kohonen.Displ();

            // _ai.QValDisp();
            _qLearning.QValDisp();
        }

        public void AiLearningRunMultileMaps()
        {
            // BEGIN run parameters
            const int iterations = 40;
            const int innerInterval = 100;
            KeyValuePair<string, string>[] mapLevelPairs = {
                new KeyValuePair<string, string> (Properties.Resources.Map1, Properties.Resources.Levels1),
                new KeyValuePair<string, string> (Properties.Resources.Map2, Properties.Resources.Levels2),
                new KeyValuePair<string, string> (Properties.Resources.Map3, Properties.Resources.Levels3),
                new KeyValuePair<string, string> (Properties.Resources.Map4, Properties.Resources.Levels4),
                new KeyValuePair<string, string> (Properties.Resources.Map5, Properties.Resources.Levels5)
            };
            // END run parameters

            // BEGIN preparation
            var aiAdapter = (KohonenGameStateManagerSemiInteligentActions)_aiAdapter;
            aiAdapter.SetRewardMultiplier(1.0 / 10000);
            // END preparation

            foreach (var mapLevel in mapLevelPairs)
            {
                Console.WriteLine(@"Running with map '{0}' (start time: {1})", mapLevel.Key, DateTime.Now);
                for (var i = 1; i <= iterations; ++i)
                {
                    _iteration = i;
                    var gamesWon = 0;
                    var gamesLost = 0;

                    // TODO edit learning rate

                    for (var j = 0; j < innerInterval; ++j)
                    {
                        if (GameState.Won == SingleAiLongRunIteration(mapLevel.Key, mapLevel.Value))
                        {
                            if (_firstGameWonLevel == -1)
                                _firstGameWonLevel = i * innerInterval + j;
                            ++gamesWon;
                        }
                        else
                        {
                            ++gamesLost;
                        }
                    }

                    _won += gamesWon;
                    Console.WriteLine(@"  Iteration {0}: won {1}, lost {2}", i, gamesWon, gamesLost);
                }
                Console.WriteLine(@"Running with map '{0}' (finish time: {1})", mapLevel.Key, DateTime.Now);
            }

            _qLearning.Save("qlearning.dat");
            _kohonen.Save("kohonen.dat");
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

        private GameState SingleAiLongRunIteration(string map, string levels)
        {
            PrepareGame(map, levels);

            while (_game.State == GameState.Waiting)
            {
                StartAiDrivenTurn();
            }

            return _game.State;
        }
    }
}
