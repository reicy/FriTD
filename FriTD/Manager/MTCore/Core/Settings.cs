using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.MTCore.Core
{
    public class Settings
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
