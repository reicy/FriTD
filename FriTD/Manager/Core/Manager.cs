using System.Diagnostics;
using Manager.Core.Delayers;
using TD.Core;
using TD.Enums;


namespace Manager.Core
{
    public class Manager
    {
        private TDGame _game;
        //private DataStore _store;
        public DataStore _store { get; set; }
        private IDelayer _delayer;

        public Manager()
        {
            _store = new DataStore();
            //_delayer = new LearningDelayer();
            _delayer = new LearningDelayer();
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

      /*  public void StartTurn(AiAction action)
        {
            _game.StartLevel();
            ExecuteLevel();
        }*/


        // ----------- unity hack begin ------------
        public void UnityStartLevel()
        {
            if (_game.State != GameStates.InProgress)
            {
                _game.StartLevel();
                _store.ExchangeData(_game.GameVisualImage());
            }
        }

        public void UnityTic()
        {
            if (_game.State == GameStates.InProgress)
            {
                _game.Tic();
                _store.ExchangeData(_game.GameVisualImage());
            }
        }
        // ----------- unity hack end -------------
        

        private void ExecuteLevel()
        {
            while (_game.State == GameStates.InProgress)
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
    }
}
