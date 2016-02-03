using System;
using System.Diagnostics;
using System.Windows.Forms;
using Manager.Core;

namespace Gui
{
    public partial class MainForm : Form
    {

        private Manager.Core.Manager _manager;

        public MainForm()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            
            switch (listBox1.SelectedIndex)
            {
                case 0:
                    _manager = ManagerBuilder.BuildAiLearningManager();
                    _manager.PrepareGame();
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    _manager = ManagerBuilder.BuildSimplePlayerManager();
                    _manager.PrepareGame();
                    break;
                default:
                    _manager = ManagerBuilder.BuildSimplePlayerManager();
                    Debug.WriteLine("Started");
                    _manager.PrepareGame();
                    break;

            }



        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_manager.IsAiMode())
            {
                _manager.StartAiDrivenTurn();
            }
            else
            {
                _manager.StartTurn();
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _manager.ExecuteCmd(textBox1.Text);
        }
    }
}
