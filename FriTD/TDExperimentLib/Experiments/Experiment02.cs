using System;
using System.Collections.Generic;
using Manager.MTCore.Core;

namespace TDExperimentLib.Experiments
{
    public class Experiment02 : ExperimentBase
    {
        public Experiment02() : base("All maps at once")
        {
        }

        public override KeyValuePair<bool, string> SetData(Dictionary<string, object> propVals)
        {
            return new KeyValuePair<bool, string>(true, "");
        }

        public override void Run()
        {
            Console.WriteLine(@"Zmačkol som MT experimenty");
            var manager = new MtManager();
            manager.ExperimentRun1();
        }
    }
}
