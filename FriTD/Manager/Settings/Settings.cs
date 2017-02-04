using System;
using System.Collections.Generic;

namespace Manager.Settings
{
    public class Settings : IDataStructure
    {
        public List<string> maps { get; set; } = new List<string>();
        public int numberOfIterationsPerMap { get; set; }
        public int numberOfIterationsPerMapWithKohonen { get; set; }
        public bool useCosDist { get; set; }
        public double qLearningRandomActionProbability { get; set; }
        public double qLearningDiscountFactor { get; set; }
        public double qLearningLearningRate { get; set; }
        public int kohonenRows { get; set; }
        public int kohonenCols { get; set; }
        public double kohonenRadius { get; set; }
        public double kohonenLearningRate { get; set; }
        public double kohonenDistFactor { get; set; }
        public string kohonenLoadFile { get; set; }
        public string qLearningLoadFile { get; set; }
        public string kohonenSaveFile { get; set; }
        public string qLearningSaveFile { get; set; }

        public Settings()
        {
            ResetToDefault();
        }

        public void ResetToDefault(bool resetMapsToo = true)
        {
            if (resetMapsToo) maps = new List<string>();
            numberOfIterationsPerMap = 3000;
            numberOfIterationsPerMapWithKohonen = 3000;
            useCosDist = false;
            qLearningRandomActionProbability = 0.3;
            qLearningDiscountFactor = 1;
            qLearningLearningRate = 0.5;
            kohonenRows = 30;
            kohonenCols = 30;
            kohonenRadius = 2;
            kohonenLearningRate = 0.5;
            kohonenDistFactor = 1;
            kohonenLoadFile = null;
            qLearningLoadFile = null;
            kohonenSaveFile = null;
            qLearningSaveFile = null;
        }

        public override string ToString()
        {
            string m = "[";
            foreach (var map in maps)
            {
                m += map + " ";
            }
            m += "]";
            return $"maps: {m}, numberOfIterationsPerMap: {numberOfIterationsPerMap}, numberOfIterationsPerMapWithKohonen: {numberOfIterationsPerMapWithKohonen}, useCosDist: {useCosDist}, qLearningRandomActionProbability: {qLearningRandomActionProbability}, qLearningDiscountFactor: {qLearningDiscountFactor}, qLearningLearningRate: {qLearningLearningRate}, kohonenRows: {kohonenRows}, kohonenCols: {kohonenCols}, kohonenRadius: {kohonenRadius}, kohonenLearningRate: {kohonenLearningRate}, kohonenDistFactor: {kohonenDistFactor}, kohonenLoadFile: {kohonenLoadFile}, qLearningLoadFile: {qLearningLoadFile}, kohonenSaveFile: {kohonenSaveFile}, qLearningSaveFile: {qLearningSaveFile}";
        }

        public KeyValuePair<bool, string> SetData(Dictionary<string, object> propVals)
        {
            int tmpNumberOfIterationsPerMap;
            int tmpNumberOfIterationsPerMapWithKohonen;
            bool tmpUseCosDist;
            double tmpQLearningRandomActionProbability;
            double tmpQLearningDiscountFactor;
            double tmpQLearningLearningRate;
            int tmpKohonenRows;
            int tmpKohonenCols;
            double tmpKohonenRadius;
            double tmpKohonenLearningRate;
            double tmpKohonenDistFactor;
            string tmpKohonenLoadFile;
            string tmpQLearningLoadFile;
            string tmpKohonenSaveFile;
            string tmpQLearningSaveFile;

            var tmp = propVals["maps"].ToString();
            var items = tmp.Substring(1, tmp.Length - 2).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var ok = int.TryParse(propVals["numberOfIterationsPerMap"].ToString(), out tmpNumberOfIterationsPerMap);
            if (!ok) return new KeyValuePair<bool, string>(false, "The 'numberOfIterationsPerMap' property has invalid value!");
            ok = int.TryParse(propVals["numberOfIterationsPerMapWithKohonen"].ToString(), out tmpNumberOfIterationsPerMapWithKohonen);
            if (!ok) return new KeyValuePair<bool, string>(false, "The 'numberOfIterationsPerMapWithKohonen' property has invalid value!");
            ok = bool.TryParse(propVals["useCosDist"].ToString(), out tmpUseCosDist);
            if (!ok) return new KeyValuePair<bool, string>(false, "The 'useCosDist' property has invalid value!");
            ok = double.TryParse(propVals["qLearningRandomActionProbability"].ToString(), out tmpQLearningRandomActionProbability);
            if (!ok) return new KeyValuePair<bool, string>(false, "The 'qLearningRandomActionProbability' property has invalid value!");
            ok = double.TryParse(propVals["qLearningDiscountFactor"].ToString(), out tmpQLearningDiscountFactor);
            if (!ok) return new KeyValuePair<bool, string>(false, "The 'qLearningDiscountFactor' property has invalid value!");
            ok = double.TryParse(propVals["qLearningLearningRate"].ToString(), out tmpQLearningLearningRate);
            if (!ok) return new KeyValuePair<bool, string>(false, "The 'qLearningLearningRate' property has invalid value!");
            ok = int.TryParse(propVals["kohonenRows"].ToString(), out tmpKohonenRows);
            if (!ok) return new KeyValuePair<bool, string>(false, "The 'kohonenRows' property has invalid value!");
            ok = int.TryParse(propVals["kohonenCols"].ToString(), out tmpKohonenCols);
            if (!ok) return new KeyValuePair<bool, string>(false, "The 'kohonenCols' property has invalid value!");
            ok = double.TryParse(propVals["kohonenRadius"].ToString(), out tmpKohonenRadius);
            if (!ok) return new KeyValuePair<bool, string>(false, "The 'kohonenRadius' property has invalid value!");
            ok = double.TryParse(propVals["kohonenLearningRate"].ToString(), out tmpKohonenLearningRate);
            if (!ok) return new KeyValuePair<bool, string>(false, "The 'kohonenLearningRate' property has invalid value!");
            ok = double.TryParse(propVals["kohonenDistFactor"].ToString(), out tmpKohonenDistFactor);
            if (!ok) return new KeyValuePair<bool, string>(false, "The 'kohonenDistFactor' property has invalid value!");
            tmpKohonenLoadFile = propVals["kohonenLoadFile"]?.ToString();
            tmpQLearningLoadFile = propVals["qLearningLoadFile"]?.ToString();
            tmpKohonenSaveFile = propVals["kohonenSaveFile"]?.ToString();
            tmpQLearningSaveFile = propVals["qLearningSaveFile"]?.ToString();

            numberOfIterationsPerMap = tmpNumberOfIterationsPerMap;
            numberOfIterationsPerMapWithKohonen = tmpNumberOfIterationsPerMapWithKohonen;
            useCosDist = tmpUseCosDist;
            qLearningRandomActionProbability = tmpQLearningRandomActionProbability;
            qLearningDiscountFactor = tmpQLearningDiscountFactor;
            qLearningLearningRate = tmpQLearningLearningRate;
            kohonenRows = tmpKohonenRows;
            kohonenCols = tmpKohonenCols;
            kohonenRadius = tmpKohonenRadius;
            kohonenLearningRate = tmpKohonenLearningRate;
            kohonenDistFactor = tmpKohonenDistFactor;
            kohonenLoadFile = tmpKohonenLoadFile;
            qLearningLoadFile = tmpQLearningLoadFile;
            kohonenSaveFile = tmpKohonenSaveFile;
            qLearningSaveFile = tmpQLearningSaveFile;
            maps = new List<string>();
            foreach (var item in items)
            {
                maps.Add(item.Trim());
            }

            return new KeyValuePair<bool, string>(true, "");
        }

        public string ToCSVString()
        {
            string m = "[";
            foreach (var map in maps)
            {
                m += map + " ";
            }
            m += "]";
            return $"{m}, {numberOfIterationsPerMap}, {numberOfIterationsPerMapWithKohonen}, {useCosDist}, {qLearningRandomActionProbability}, {qLearningDiscountFactor}, {qLearningLearningRate}, {kohonenRows}, {kohonenCols}, {kohonenRadius}, {kohonenLearningRate}, {kohonenDistFactor}, {kohonenLoadFile}, {qLearningLoadFile}, {kohonenSaveFile}, {qLearningSaveFile}";
        }
    }
}
