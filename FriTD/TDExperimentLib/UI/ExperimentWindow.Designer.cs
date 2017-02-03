namespace TDExperimentLib.UI
{
    partial class ExperimentWindow
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.richTextBoxOutput = new System.Windows.Forms.RichTextBox();
            this.groupBoxConsoleOutput = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.buttonClearOutput = new System.Windows.Forms.Button();
            this.buttonRunExperiment = new System.Windows.Forms.Button();
            this.buttonChangeFont = new System.Windows.Forms.Button();
            this.buttonSaveOutput = new System.Windows.Forms.Button();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.buttonSaveProperties = new System.Windows.Forms.Button();
            this.buttonLoadProperties = new System.Windows.Forms.Button();
            this.dataGridViewProperties = new System.Windows.Forms.DataGridView();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.groupBoxConsoleOutput.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.groupBoxSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBoxOutput
            // 
            this.richTextBoxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxOutput.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.richTextBoxOutput.Location = new System.Drawing.Point(3, 3);
            this.richTextBoxOutput.Name = "richTextBoxOutput";
            this.richTextBoxOutput.ReadOnly = true;
            this.richTextBoxOutput.Size = new System.Drawing.Size(729, 478);
            this.richTextBoxOutput.TabIndex = 0;
            this.richTextBoxOutput.Text = "";
            // 
            // groupBoxConsoleOutput
            // 
            this.groupBoxConsoleOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxConsoleOutput.Controls.Add(this.tabControl1);
            this.groupBoxConsoleOutput.Controls.Add(this.buttonClearOutput);
            this.groupBoxConsoleOutput.Controls.Add(this.buttonRunExperiment);
            this.groupBoxConsoleOutput.Controls.Add(this.buttonChangeFont);
            this.groupBoxConsoleOutput.Controls.Add(this.buttonSaveOutput);
            this.groupBoxConsoleOutput.Location = new System.Drawing.Point(12, 12);
            this.groupBoxConsoleOutput.Name = "groupBoxConsoleOutput";
            this.groupBoxConsoleOutput.Size = new System.Drawing.Size(755, 563);
            this.groupBoxConsoleOutput.TabIndex = 1;
            this.groupBoxConsoleOutput.TabStop = false;
            this.groupBoxConsoleOutput.Text = "Output";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(6, 19);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(743, 510);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.richTextBoxOutput);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(735, 484);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Console";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.chart);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(735, 484);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Chart";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // chart
            // 
            chartArea1.Name = "ChartArea1";
            this.chart.ChartAreas.Add(chartArea1);
            this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chart.Legends.Add(legend1);
            this.chart.Location = new System.Drawing.Point(3, 3);
            this.chart.Name = "chart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart.Series.Add(series1);
            this.chart.Size = new System.Drawing.Size(729, 478);
            this.chart.TabIndex = 0;
            this.chart.Text = "chart1";
            // 
            // buttonClearOutput
            // 
            this.buttonClearOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClearOutput.Location = new System.Drawing.Point(392, 535);
            this.buttonClearOutput.Name = "buttonClearOutput";
            this.buttonClearOutput.Size = new System.Drawing.Size(115, 23);
            this.buttonClearOutput.TabIndex = 4;
            this.buttonClearOutput.Text = "Clear output";
            this.buttonClearOutput.UseVisualStyleBackColor = true;
            this.buttonClearOutput.Click += new System.EventHandler(this.buttonClearOutput_Click);
            // 
            // buttonRunExperiment
            // 
            this.buttonRunExperiment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRunExperiment.Location = new System.Drawing.Point(6, 534);
            this.buttonRunExperiment.Name = "buttonRunExperiment";
            this.buttonRunExperiment.Size = new System.Drawing.Size(114, 23);
            this.buttonRunExperiment.TabIndex = 3;
            this.buttonRunExperiment.Text = "Run experiment";
            this.buttonRunExperiment.UseVisualStyleBackColor = true;
            this.buttonRunExperiment.Click += new System.EventHandler(this.buttonRunExperiment_Click);
            // 
            // buttonChangeFont
            // 
            this.buttonChangeFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonChangeFont.Location = new System.Drawing.Point(513, 534);
            this.buttonChangeFont.Name = "buttonChangeFont";
            this.buttonChangeFont.Size = new System.Drawing.Size(115, 23);
            this.buttonChangeFont.TabIndex = 2;
            this.buttonChangeFont.Text = "Change font";
            this.buttonChangeFont.UseVisualStyleBackColor = true;
            this.buttonChangeFont.Click += new System.EventHandler(this.buttonChangeFont_Click);
            // 
            // buttonSaveOutput
            // 
            this.buttonSaveOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveOutput.Location = new System.Drawing.Point(634, 534);
            this.buttonSaveOutput.Name = "buttonSaveOutput";
            this.buttonSaveOutput.Size = new System.Drawing.Size(115, 23);
            this.buttonSaveOutput.TabIndex = 1;
            this.buttonSaveOutput.Text = "Save output to file";
            this.buttonSaveOutput.UseVisualStyleBackColor = true;
            this.buttonSaveOutput.Click += new System.EventHandler(this.buttonSaveOutput_Click);
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSettings.Controls.Add(this.buttonSaveProperties);
            this.groupBoxSettings.Controls.Add(this.buttonLoadProperties);
            this.groupBoxSettings.Controls.Add(this.dataGridViewProperties);
            this.groupBoxSettings.Location = new System.Drawing.Point(12, 12);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(301, 564);
            this.groupBoxSettings.TabIndex = 2;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Properties";
            // 
            // buttonSaveProperties
            // 
            this.buttonSaveProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveProperties.Location = new System.Drawing.Point(139, 535);
            this.buttonSaveProperties.Name = "buttonSaveProperties";
            this.buttonSaveProperties.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveProperties.TabIndex = 2;
            this.buttonSaveProperties.Text = "Save";
            this.buttonSaveProperties.UseVisualStyleBackColor = true;
            this.buttonSaveProperties.Click += new System.EventHandler(this.buttonSaveProperties_Click);
            // 
            // buttonLoadProperties
            // 
            this.buttonLoadProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLoadProperties.Location = new System.Drawing.Point(220, 535);
            this.buttonLoadProperties.Name = "buttonLoadProperties";
            this.buttonLoadProperties.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadProperties.TabIndex = 1;
            this.buttonLoadProperties.Text = "Load";
            this.buttonLoadProperties.UseVisualStyleBackColor = true;
            this.buttonLoadProperties.Click += new System.EventHandler(this.buttonLoadProperties_Click);
            // 
            // dataGridViewProperties
            // 
            this.dataGridViewProperties.AllowUserToAddRows = false;
            this.dataGridViewProperties.AllowUserToDeleteRows = false;
            this.dataGridViewProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewProperties.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewProperties.Location = new System.Drawing.Point(6, 19);
            this.dataGridViewProperties.MultiSelect = false;
            this.dataGridViewProperties.Name = "dataGridViewProperties";
            this.dataGridViewProperties.RowHeadersVisible = false;
            this.dataGridViewProperties.Size = new System.Drawing.Size(289, 510);
            this.dataGridViewProperties.TabIndex = 0;
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.groupBoxSettings);
            this.splitContainer.Panel1MinSize = 100;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.groupBoxConsoleOutput);
            this.splitContainer.Panel2MinSize = 150;
            this.splitContainer.Size = new System.Drawing.Size(1108, 587);
            this.splitContainer.SplitterDistance = 325;
            this.splitContainer.TabIndex = 3;
            // 
            // ExperimentWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1108, 587);
            this.Controls.Add(this.splitContainer);
            this.Name = "ExperimentWindow";
            this.Text = "ExperimentWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExperimentWindow_FormClosing);
            this.groupBoxConsoleOutput.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.groupBoxSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProperties)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxOutput;
        private System.Windows.Forms.GroupBox groupBoxConsoleOutput;
        private System.Windows.Forms.Button buttonSaveOutput;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Button buttonChangeFont;
        private System.Windows.Forms.Button buttonSaveProperties;
        private System.Windows.Forms.Button buttonLoadProperties;
        private System.Windows.Forms.DataGridView dataGridViewProperties;
        private System.Windows.Forms.Button buttonRunExperiment;
        private System.Windows.Forms.Button buttonClearOutput;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
    }
}