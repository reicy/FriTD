using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manager.MTCore.Core;
using Manager.Utils;

namespace TDExperimentLib.Experiments
{
    public class Experiment05 : ExperimentBase
    {
        private MtManager manager = new MtManager();
        private Settings s = new Settings();
        private string cvsFilename = "results.csv";

        public Experiment05() : base("Exp")
        {
        }

        public override KeyValuePair<bool, string> SetData(Dictionary<string, object> propVals)
        {
            return new KeyValuePair<bool, string>(true, "");
        }

        public override void Run()
        {
            CustomLogger.logToFile = true;

            var prefix = "test1_";
            RunMap("Map", prefix: prefix);
            RunMap("Map1", prefix: prefix);

            prefix = "test2_";
            RunMap("Map3", prefix: prefix);
            RunMap("Map4", prefix: prefix);
        }

        private void RunMap(string map, int numOfThreads = 4, string prefix = "")
        {
            s.ResetToDefault();
            s.maps.Clear();
            for (int i = 0; i < numOfThreads; i++)
            {
                s.maps.Add(map);
            }
            s.maps.Add("Map5");
            s.numberOfIterationsPerMap = 100;
            s.numberOfIterationsPerMapWithKohonen = 100;

            s.kohonenSaveFile = prefix + map + "_koh.txt";
            s.qLearningSaveFile = prefix + map + "_q.txt";
            s.kohonenLoadFile = null;
            s.qLearningLoadFile = null;

            CustomLogger.filename = prefix + map + "_stats.txt";
            CustomLogger.ClearActualLogFile();

            s.kohonenRadius = 3;
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCVSString(), cvsFilename);

            s.kohonenLoadFile = s.kohonenSaveFile;
            s.qLearningLoadFile = s.qLearningSaveFile;

            s.kohonenLearningRate = 0.3;
            s.kohonenRadius = 2;
            manager.ExperimentRun(s);
            CustomLogger.LogToFile(s.ToCSVString() + ", " + MtStats.ToCVSString(), cvsFilename);
        }
    }
}
