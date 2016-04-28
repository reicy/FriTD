using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Configuration;
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
                Console.WriteLine("run num: "+i);
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

            Console.WriteLine("count: {0} states: {1} first game won: {2} won: {3} lost: {4}",Statistics.counter, Statistics.endgameStatesNum*1.0/Statistics.counter, Statistics.firstWonRound * 1.0 / Statistics.gamesWithVictory, Statistics.gamesWon * 1.0 / Statistics.counter, Statistics.gamesLost * 1.0 / Statistics.counter);

        }

        internal void InsertAi(StreamReader reader)
        {
            _ai = new AICore(0.1, 1, 0.5,reader);
            _aiAdapter = new GameStateManager(_ai);
            
        }

        public bool IsAiMode()
        {
            return _ai != null;
        }

        public void PrepareGame()
        {
            _game = new TDGame();
            _game.InitGame(Properties.Resources.Towers, Properties.Resources.Map, Properties.Resources.Enemies, Properties.Resources.Levels);
            
           
        }

        public void StartTurn()
        {
            _game.StartLevel();
            ExecuteLevel();

        }


        public void InsertAi()
        {
            _ai = new AICore(0.1,1,0.5);
           // _aiAdapter = new GameStateManager(_ai);
            Console.WriteLine("AI inserted");
            _qLearning = new QLearning<KohonenAiState>(0.4, 1, 0.5);
           // _aiAdapter = new NewGameStateManager(_qLearning);
            KohonenCore<StateVector> kohonen = new KohonenCore<StateVector>(50, 50, 3, 0.5, 1, 1, 0.5);
            _aiAdapter = new KohonenGameStateManager(_qLearning,kohonen);
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

                var tmp = _game.GameVisualImage();
                //Debug.WriteLine(tmp.Hp);
                //Debug.WriteLine(tmp.Enemies.Count);
                if (tmp.Enemies.Count > 0)
                {
                    //Debug.WriteLine(tmp.Enemies[0].X +" "+ tmp.Enemies[0].Y );
                }
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
            var decisionResult = _aiAdapter.ExecuteDecision(_game.GameStateImage());
            var arr = decisionResult.Split(' ');
            foreach (var cmd in arr)
            {
                if(cmd.Length > 0)ExecuteCmd(cmd);
            }

            StartTurn();

            _aiAdapter.ExecuteReward(_game.GameStateImage());


        }

        public void AiLearningRun()
        {
            int won = 0;
            int lost = 0;

            int innerInterval = 100;
            int iterations = 1000;

            for (int i = 0; i < iterations; i++)
            {

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
                }
               // _ai.saveQ_valuesToFile(@"C:\Users\Tomas\Desktop\copy\"+i);
                this.won += won;
                Console.WriteLine("Iteration: "+i+" won: "+won+" lost: "+lost);
                won = 0;
                lost = 0;
            }
            
           // _ai.QValDisp();
          // _qLearning.QValDisp();

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

            return _game.State;
        }

      


    }


   



}
