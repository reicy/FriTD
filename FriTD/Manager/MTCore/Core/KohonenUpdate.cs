using Manager.Kohonen;

namespace Manager.MTCore.Core
{
    public class KohonenUpdate
    {
        public StateVector Vector { get; set; }
        public int Col { get; set; }
        public int Row { get; set; }
    }
}