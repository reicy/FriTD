using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Manager.Core;
using Manager.Kohonen;
using Manager.MTCore;

namespace Gui
{
    public partial class MainForm : Form
    {

        private Manager.Core.Manager _manager;

        public MainForm()
        {
            InitializeComponent();
            kc = new KohonenCore<RgbVector>(50, 50, 3, 0.5, 1, 1, 0.5, false);
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


        private KohonenCore<RgbVector> kc;

        private void button4_Click(object sender, EventArgs e)
        {
       
            //  kc.ReArrange(10,10,new RgbVector(100,0,0));
            //   kc.ReArrange(13, 10, new RgbVector(0, 168, 0));
            System.Drawing.Graphics graphics = this.CreateGraphics();
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(200, 200, 400, 400);

            RgbVector c;
            Random rnd = new Random();
            int[] dim;
            for (int l = 0; l < 10; l++)
            {
               if(l%1000 == 0)Console.WriteLine(l);
                
          /*      switch (rnd.Next(3))
                {
                    case 0:
                        c = new RgbVector(rnd.Next(255), 0,0);
                        break;
                    case 1:
                        c = new RgbVector(0, rnd.Next(255),0);
                        break;
                    default:
                        c = new RgbVector(0,0, rnd.Next(255));
                        break;
                }*/
                c = new RgbVector(/*rnd.Next(255)*/0, rnd.Next(255), rnd.Next(255));//rnd.Next(255),rnd.Next(255));
                dim = kc.Winner(c);
                kc.ReArrange(dim[0],dim[1],c);

            }
          //  kc.Displ();

           
           // graphics.DrawEllipse(System.Drawing.Pens.Black, rectangle);
           // graphics.DrawRectangle(System.Drawing.Pens.Red, rectangle);
//            graphics.FillRectangle(new SolidBrush(new Color()), new Rectangle() );

            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    graphics.FillRectangle(new SolidBrush(Color.FromArgb(255,kc[i,j][0], kc[i, j][1], kc[i, j][2])), new Rectangle(200+i*4, 200+j*4, 4, 4));
                }
            }

         //   graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, kc[49, 49][0], kc[49, 49][1], kc[49, 49][2])), new Rectangle(200 + 60 * 5, 200 + 60 * 5, 10, 10));

          //  Console.WriteLine(kc[49, 49][0] + " " + kc[49, 49][1] + " " + kc[49, 49][2]);
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
            MtManager manager = new MtManager();
            manager.ProcessLearning();
        }
    }
}
