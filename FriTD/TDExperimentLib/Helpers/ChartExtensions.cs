using System.ComponentModel;
using System.Windows.Forms;

namespace TDExperimentLib.Helpers
{
    public static class ChartExtensions
    {
        public static void InvokeIfRequired(this ISynchronizeInvoke obj, MethodInvoker action)
        {
            if (obj.InvokeRequired)
                obj.Invoke(action, new object[] { });
            else
                action();
        }
    }
}