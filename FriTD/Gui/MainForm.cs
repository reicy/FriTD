using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    _manager = ManagerBuilder.BuildSimplePlayerManager();
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
            _manager.StartTurn();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _manager.ExecuteCmd(textBox1.Text);
        }
    }
}
