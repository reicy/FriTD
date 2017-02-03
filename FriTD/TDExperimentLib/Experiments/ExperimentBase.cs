using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;

namespace TDExperimentLib.Experiments
{
    public abstract class ExperimentBase
    {
        private readonly string _name;

        protected ExperimentBase(string name)
        {
            _name = name;
        }

        public string GetName()
        {
            return _name;
        }

        public virtual void SetChart(Chart chart)
        {

        }

        public abstract KeyValuePair<bool, string> SetData(Dictionary<string, object> propVals);

        public void Start()
        {
            OnStart?.Invoke();
            Run();
            OnFinish?.Invoke();
        }

        public abstract void Run();

        public delegate void BeforeAfterAction();

        public event BeforeAfterAction OnStart;
        public event BeforeAfterAction OnFinish;
    }
}
