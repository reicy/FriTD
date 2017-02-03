using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace Manager
{
    public interface IChartWrapper
    {
        void InitChart();

        void ClearPoints(int series = 0);

        void ClearAllPoints();

        void ClearSeries();

        void AddXY(double x, double y, int series = 0);

        void AddY(double y, int series = 0);

        int AddSeries(string name, SeriesChartType type);

        int AddSeries(string name, SeriesChartType type, Color color);
    }
}
