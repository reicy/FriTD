using System.Threading;

namespace Manager.Delayers
{
    public class SimpleDelayer : IDelayer
    {
        public void Delay()
        {
            Thread.Sleep(100);
        }
    }
}