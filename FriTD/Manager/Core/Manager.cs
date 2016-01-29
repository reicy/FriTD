using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manager.Core.Delayers;
using TD.Core;
using TD.Enums;


namespace Manager.Core
{
    public class Manager
    {
        private TDGame _game;
        private DataStore _store;
        private IDelayer _delayer;

        public void PrepareGame()
        {
            _game = new TDGame();
        }

        public void StartTurn()
        {
            
        }

      /*  public void StartTurn(AiAction action)
        {
            _game.StartLevel();
            ExecuteLevel();
        }*/

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
    }
}
