﻿namespace Gui
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.LongRun = new System.Windows.Forms.Button();
            this.MTStartButton = new System.Windows.Forms.Button();
            this.buttonLongRunMoreMaps = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(26, 141);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(125, 30);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start Game";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "1 Ai learning mode(single run)",
            "2 Ai learning mode(long run)",
            "3 Player mode",
            "4 Simple player mode"});
            this.listBox1.Location = new System.Drawing.Point(26, 45);
            this.listBox1.Margin = new System.Windows.Forms.Padding(2);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(167, 82);
            this.listBox1.TabIndex = 1;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(26, 311);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(148, 32);
            this.button2.TabIndex = 2;
            this.button2.Text = "Exec cmd";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(26, 280);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(149, 20);
            this.textBox1.TabIndex = 3;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(26, 362);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(126, 32);
            this.button3.TabIndex = 4;
            this.button3.Text = "Start turn";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(344, 69);
            this.button4.Margin = new System.Windows.Forms.Padding(2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(56, 19);
            this.button4.TabIndex = 5;
            this.button4.Text = "Test color";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // LongRun
            // 
            this.LongRun.Location = new System.Drawing.Point(458, 69);
            this.LongRun.Margin = new System.Windows.Forms.Padding(2);
            this.LongRun.Name = "LongRun";
            this.LongRun.Size = new System.Drawing.Size(82, 25);
            this.LongRun.TabIndex = 6;
            this.LongRun.Text = "LongRun";
            this.LongRun.UseVisualStyleBackColor = true;
            this.LongRun.Click += new System.EventHandler(this.LongRun_Click);
            // 
            // MTStartButton
            // 
            this.MTStartButton.Location = new System.Drawing.Point(558, 69);
            this.MTStartButton.Margin = new System.Windows.Forms.Padding(2);
            this.MTStartButton.Name = "MTStartButton";
            this.MTStartButton.Size = new System.Drawing.Size(97, 25);
            this.MTStartButton.TabIndex = 7;
            this.MTStartButton.Text = "MT";
            this.MTStartButton.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.MTStartButton.UseVisualStyleBackColor = true;
            this.MTStartButton.Click += new System.EventHandler(this.MTStartButton_Click);
            // 
            // buttonLongRunMoreMaps
            // 
            this.buttonLongRunMoreMaps.Location = new System.Drawing.Point(458, 100);
            this.buttonLongRunMoreMaps.Name = "buttonLongRunMoreMaps";
            this.buttonLongRunMoreMaps.Size = new System.Drawing.Size(82, 45);
            this.buttonLongRunMoreMaps.TabIndex = 8;
            this.buttonLongRunMoreMaps.Text = "LongRun (more maps)";
            this.buttonLongRunMoreMaps.UseVisualStyleBackColor = true;
            this.buttonLongRunMoreMaps.Click += new System.EventHandler(this.buttonLongRunMoreMaps_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(558, 98);
            this.button5.Margin = new System.Windows.Forms.Padding(2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(97, 25);
            this.button5.TabIndex = 9;
            this.button5.Text = "MT - experiment";
            this.button5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 641);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.buttonLongRunMoreMaps);
            this.Controls.Add(this.MTStartButton);
            this.Controls.Add(this.LongRun);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "FriTD";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button LongRun;
        private System.Windows.Forms.Button MTStartButton;
        private System.Windows.Forms.Button buttonLongRunMoreMaps;
        private System.Windows.Forms.Button button5;
    }
}

