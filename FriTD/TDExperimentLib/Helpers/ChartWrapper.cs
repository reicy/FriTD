using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using Manager;

namespace TDExperimentLib.Helpers
{
    public class ChartWrapper : IChartWrapper
    {
        public static readonly Color[] Colors =
        {
            Color.Blue, Color.Red, Color.Green, Color.Purple, Color.Brown, Color.Orange, Color.DarkCyan
        };

        public Chart Chart { get; }

        public ChartWrapper(Chart chart)
        {
            Chart = chart;
        }

        public void InitChart()
        {
            Chart.InvokeIfRequired(() => Chart.Series.Clear());
            var series = new Series(Chart.Name) { ChartType = SeriesChartType.Line, IsVisibleInLegend = false };
            Chart.InvokeIfRequired(() => Chart.Series.Add(series));
        }

        public void ClearPoints(int series = 0)
        {
            Chart.InvokeIfRequired(() => Chart.Series[series].Points.Clear());
        }

        public void ClearAllPoints()
        {
            Chart.InvokeIfRequired(() =>
            {
                foreach (var series in Chart.Series)
                {
                    series.Points.Clear();
                }
            });
        }

        public void ClearSeries()
        {
            Chart.InvokeIfRequired(() => Chart.Series.Clear());
        }

        public int AddSeries(string name, SeriesChartType type)
        {
            var cnt = 0;
            Chart.InvokeIfRequired(() => cnt = Chart.Series.Count);
            var clr = Colors[cnt];
            return AddSeries(name, type, clr);
        }

        public int AddSeries(string name, SeriesChartType type, Color color)
        {
            var series = new Series
            {
                Name = name,
                ChartType = type,
                Color = color
            };
            var val = 0;
            Chart.InvokeIfRequired(() =>
            {
                Chart.Series.Add(series);
                val = Chart.Series.Count - 1;
            });
            return val;
        }

        public void AddXY(double x, double y, int series = 0)
        {
            Chart.InvokeIfRequired(() => Chart.Series[series].Points.AddXY(x, y));
        }

        public void AddY(double y, int series = 0)
        {
            Chart.InvokeIfRequired(() => Chart.Series[series].Points.AddY(y));
        }
    }
}