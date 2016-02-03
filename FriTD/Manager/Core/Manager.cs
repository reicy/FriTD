using System.ComponentModel;
using System.Diagnostics;
using System.Net.Configuration;
using Manager.AI;
using Manager.Core.Delayers;
using Manager.GameStates;
using TD.Core;
using TD.Enums;


namespace Manager.Core
{
    public class Manager
    {
        private TDGame _game;
        private GameStateManager _aiAdapter;
        private AICore _ai; 

        //private DataStore _store;
        public DataStore _store { get; set; }
        private readonly IDelayer _delayer;

        public Manager()
        {
            _store = new DataStore();
            //_delayer = new SimpleDelayer();
            _delayer = new LearningDelayer();

           
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
            _ai = new AICore(1,1,1);
            _aiAdapter = new GameStateManager(_ai);
        }



        // ----------- unity hack begin ------------
        public void UnityStartLevel()
        {
            if (_game.State != GameState.InProgress)
            {
                _game.StartLevel();
                _store.ExchangeData(_game.GameVisualImage());
            }
        }

        public void UnityTic()
        {
            if (_game.State == GameState.InProgress)
            {
                _game.Tic();
                _store.ExchangeData(_game.GameVisualImage());
            }
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
                Debug.WriteLine(tmp.Hp);
                Debug.WriteLine(tmp.Enemies.Count);
                if (tmp.Enemies.Count > 0)
                {
                    Debug.WriteLine(tmp.Enemies[0].X +" "+ tmp.Enemies[0].Y );
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
    }
}
