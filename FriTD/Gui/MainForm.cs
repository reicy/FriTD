using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Manager.Core;
using Manager.Kohonen;
using Manager.MTCore.Core;

namespace Gui
{
    public partial class MainForm : Form
    {
        private Manager.Core.Manager _manager;
        private readonly KohonenCore<RgbVector> _kc;

        public MainForm()
        {
            InitializeComponent();
            _kc = new KohonenCore<RgbVector>(50, 50, 3, 0.5, 1, 1, 0.5, false);
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
                    _manager = ManagerBuilder.BuildAiLearningManager();
                    _manager.AiLearningRun();
                    break;
                case 2:
                    _manager = ManagerBuilder.BuildAiLearningManager();
                    _manager.StatisticsRun();
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

        private void button4_Click(object sender, EventArgs e)
        {
            //_kc.ReArrange(10, 10, new RgbVector(100, 0, 0));
            //_kc.ReArrange(13, 10, new RgbVector(0, 168, 0));
            Graphics graphics = CreateGraphics();
            //Rectangle rectangle = new Rectangle(200, 200, 400, 400);

            RgbVector c;
            var rnd = new Random();
            for (int l = 0; l < 10; l++)
            {
                if (l % 1000 == 0) Console.WriteLine(l);

                /*switch (rnd.Next(3))
                {
                    case 0:
                        c = new RgbVector(rnd.Next(255), 0, 0);
                        break;
                    case 1:
                        c = new RgbVector(0, rnd.Next(255), 0);
                        break;
                    default:
                        c = new RgbVector(0, 0, rnd.Next(255));
                        break;
                }*/

                c = new RgbVector(/*rnd.Next(255)*/0, rnd.Next(255), rnd.Next(255)); // rnd.Next(255), rnd.Next(255));
                var dim = _kc.Winner(c);
                _kc.ReArrange(dim[0], dim[1], c);
            }

            //_kc.Displ();

            //graphics.DrawEllipse(Pens.Black, rectangle);
            //graphics.DrawRectangle(Pens.Red, rectangle);
            //graphics.FillRectangle(new SolidBrush(new Color()), new Rectangle());

            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, _kc[i, j][0], _kc[i, j][1], _kc[i, j][2])), new Rectangle(200 + i * 4, 200 + j * 4, 4, 4));
                }
            }

            //graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, _kc[49, 49][0], _kc[49, 49][1], _kc[49, 49][2])), new Rectangle(200 + 60 * 5, 200 + 60 * 5, 10, 10));

            //Console.WriteLine(@"{0} {1} {2}", _kc[49, 49][0], _kc[49, 49][1], _kc[49, 49][2]);
            Console.WriteLine();
        }

        private void LongRun_Click(object sender, EventArgs e)
        {
            _manager = ManagerBuilder.BuildAiLearningManager();
            _manager.AiLearningRun();
        }

        private void buttonLongRunMoreMaps_Click(object sender, EventArgs e)
        {
            _manager = ManagerBuilder.BuildAiLearningManager();
            _manager.AiLearningRunMultileMaps();
        }

        private void MTStartButton_Click(object sender, EventArgs e)
        {
            var manager = new MtManager();
            manager.ProcessLearning();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Console.WriteLine(@"Zmačkol som MT experimenty");
            var manager = new MtManager();
            manager.ExperimentRun1();
        }
    }
}
