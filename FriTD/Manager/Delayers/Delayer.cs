using System.Security.Cryptography.X509Certificates;

namespace Manager.Core.Delayers
{
    public interface IDelayer
    {
        void Delay();
    }
}