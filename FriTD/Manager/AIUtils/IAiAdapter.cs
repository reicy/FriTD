namespace Manager.AIUtils
{
    interface IAiAdapter
    {
        string ExecuteDecision(GameStateImg gameStateImage);
        void ExecuteReward(GameStateImg gameStateImage);
    }
}
