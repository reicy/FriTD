using System;
using Manager.MTCore.Core;

namespace TDExperimentLib.Experiments
{
    public class Experiment02 : ExperimentBase
    {
        public Experiment02() : base("All maps at once")
        {
        }

        public override void Run()
        {
            Console.WriteLine(@"Zmačkol som MT experimenty");
            var manager = new MtManager();
            manager.ExperimentRun1();
        }
    }
}
