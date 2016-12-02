using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TDExperimentLib.UI
{
    public class TextBoxWriter : TextWriter
    {
        private readonly RichTextBox _ctrl;

        public override Encoding Encoding => Encoding.UTF8;

        public TextBoxWriter(RichTextBox ctrl)
        {
            _ctrl = ctrl;
        }

        public override void Write(string format, params object[] arg)
        {
            base.Write(format, arg);
            if (_ctrl.InvokeRequired)
                _ctrl.Invoke((MethodInvoker)(() => _ctrl.AppendText(string.Format(format, arg))));
            else
                _ctrl.AppendText(string.Format(format, arg));
        }

        public override void Write(string value)
        {
            base.Write(value);
            if (_ctrl.InvokeRequired)
                _ctrl.Invoke((MethodInvoker)(() => _ctrl.AppendText(value)));
            else
                _ctrl.AppendText(value);
        }

        public override void Write(int value)
        {
            base.Write(value);
            if (_ctrl.InvokeRequired)
                _ctrl.Invoke((MethodInvoker)(() => _ctrl.AppendText(value.ToString())));
            else
                _ctrl.AppendText(value.ToString());
        }

        public override void WriteLine(string format, params object[] arg)
        {
            base.WriteLine(format, arg);
            if (_ctrl.InvokeRequired)
                _ctrl.Invoke((MethodInvoker)(() => _ctrl.AppendText(string.Format(format + Environment.NewLine, arg))));
            else
                _ctrl.AppendText(string.Format(format + Environment.NewLine, arg));
        }

        public override void WriteLine(string value)
        {
            base.WriteLine(value);
            if (_ctrl.InvokeRequired)
                _ctrl.Invoke((MethodInvoker)(() => _ctrl.AppendText(value + Environment.NewLine)));
            else
                _ctrl.AppendText(value + Environment.NewLine);
        }

        public override void WriteLine(int value)
        {
            base.WriteLine(value);
            if (_ctrl.InvokeRequired)
                _ctrl.Invoke((MethodInvoker)(() => _ctrl.AppendText(value + Environment.NewLine)));
            else
                _ctrl.AppendText(value + Environment.NewLine);
        }

        public override void WriteLine()
        {
            base.WriteLine();
            if (_ctrl.InvokeRequired)
                _ctrl.Invoke((MethodInvoker)(() => _ctrl.AppendText(Environment.NewLine)));
            else
                _ctrl.AppendText(Environment.NewLine);
        }
    }
}
