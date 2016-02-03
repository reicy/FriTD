using Manager.AI;
using TD.Core;

namespace Manager.GameState
{
    class GameStateManager
    {

        private AICore core;

        public GameStateImage PreviousStateImage { get; private set; }

        public GameStateManager(AICore core)
        {
            this.core = core;
        }

        public string ExecuteDecision(GameStateImage image)
        {

            return "";
        }

        public void ExecuteReward(GameStateImage image)
        {

        }
    }
}
