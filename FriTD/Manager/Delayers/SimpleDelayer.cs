using System.Threading;

namespace Manager.Core.Delayers
{
    public class SimpleDelayer:IDelayer
    {
        public void Delay()
        {
            Thread.Sleep(100);
        }
    }
}