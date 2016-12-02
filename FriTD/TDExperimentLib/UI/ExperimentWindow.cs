﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using TDExperimentLib.Experiments;

namespace TDExperimentLib.UI
{
    public partial class ExperimentWindow : Form
    {
        private readonly ExperimentBase _experiment;
        private Thread _thread;

        public ExperimentWindow(ExperimentBase experiment)
        {
            InitializeComponent();
            _experiment = experiment;
            CreateThread();
            Init();
        }

        private void Init()
        {
            Text = $@"Experiment {_experiment.GetName()}";

            _experiment.OnFinish += Finished;

            dataGridViewProperties.Columns.Add("name", "Property name");
            dataGridViewProperties.Columns[0].ReadOnly = true;
            dataGridViewProperties.Columns.Add("value", "Property value");
            ReloadProperties();
        }

        private void CreateThread()
        {
            _thread = new Thread(_experiment.Start);
        }

        private void StartThread()
        {
            Started();
            _thread.Start();
        }

        private void StopThread()
        {
            _thread.Abort();
            _thread.Join();
            Console.WriteLine(@"Experiment stopped by user.");
            Finished();
        }

        private void Started()
        {
            Console.SetOut(new TextBoxWriter(richTextBoxOutput));
            buttonRunExperiment.Text = @"Stop experiment";
            buttonLoadProperties.Enabled = false;
            buttonSaveProperties.Enabled = false;
            dataGridViewProperties.Enabled = false;
            richTextBoxOutput.Focus();
        }

        private void Finished()
        {
            buttonRunExperiment.Text = @"Run experiment";
            buttonLoadProperties.Enabled = true;
            buttonSaveProperties.Enabled = true;
            dataGridViewProperties.Enabled = true;
        }

        private void ReloadProperties()
        {
            var props = TypeDescriptor.GetProperties(_experiment);
            if (props.Count == 0) return;

            if (dataGridViewProperties.Rows.Count == 0)
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(_experiment))
                {
                    dataGridViewProperties.Rows.Add(property.DisplayName, property.GetValue(_experiment));
                }
            }
            else
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(_experiment))
                {
                    var row = dataGridViewProperties.Rows.Cast<DataGridViewRow>().First(x => x.Cells["name"].Value.ToString().Equals(property.DisplayName));
                    row.Cells["value"].Value = property.GetValue(_experiment);
                }
            }

            dataGridViewProperties.Refresh();
        }

        private bool UpdateProperties()
        {
            var dict = new Dictionary<string, object>();
            foreach (DataGridViewRow row in dataGridViewProperties.Rows)
            {
                dict.Add(row.Cells["name"].Value.ToString(), row.Cells["value"].Value);
            }

            var result = _experiment.SetData(dict);
            if (!result.Key)
            {
                MessageBox.Show(this, $@"Error - {result.Value}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void buttonClearOutput_Click(object sender, EventArgs e)
        {
            richTextBoxOutput.Clear();
        }

        private void buttonChangeFont_Click(object sender, EventArgs e)
        {
            var fntDlg = new FontDialog();
            var result = fntDlg.ShowDialog();
            if (result != DialogResult.OK)
                return;
            richTextBoxOutput.Font = fntDlg.Font;
        }

        private void buttonSaveOutput_Click(object sender, EventArgs e)
        {
            var chooseFileDlg = new SaveFileDialog();
            var result = chooseFileDlg.ShowDialog();
            if (result == DialogResult.OK)
                File.WriteAllText(chooseFileDlg.FileName, richTextBoxOutput.Text);
            MessageBox.Show(this, $@"Successfully saved file '{chooseFileDlg.FileName}'", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonRunExperiment_Click(object sender, EventArgs e)
        {
            switch (_thread.ThreadState)
            {
                case ThreadState.Running:
                case ThreadState.Suspended:
                case ThreadState.SuspendRequested:
                case ThreadState.WaitSleepJoin:
                    StopThread();
                    break;
                case ThreadState.Aborted:
                case ThreadState.Stopped:
                    if (!UpdateProperties()) return;
                    CreateThread();
                    StartThread();
                    break;
                case ThreadState.Unstarted:
                    if (!UpdateProperties()) return;
                    StartThread();
                    break;
            }
        }

        private void buttonLoadProperties_Click(object sender, EventArgs e)
        {
            var chooseFileDlg = new OpenFileDialog
            {
                CheckFileExists = true,
                Multiselect = false,
                Filter = @"JSON files(*.json)|*.json"
            };
            var result = chooseFileDlg.ShowDialog();
            if (result != DialogResult.OK)
                return;
            var fileName = chooseFileDlg.FileName;

            var json = File.ReadAllText(fileName);
            var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(json);
            _experiment.SetData(dict);
            ReloadProperties();

            MessageBox.Show(this, $@"Successfully loaded from '{fileName}'", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonSaveProperties_Click(object sender, EventArgs e)
        {
            if (!UpdateProperties()) return;

            var chooseFileDlg = new SaveFileDialog { Filter = @"JSON files(*.json)|*.json" };
            var result = chooseFileDlg.ShowDialog();
            if (result != DialogResult.OK)
                return;
            var fileName = chooseFileDlg.FileName;

            var json = new JavaScriptSerializer().Serialize(_experiment);
            File.WriteAllText(fileName, json);

            MessageBox.Show(this, $@"Successfully saved to '{fileName}'", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExperimentWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (_thread.ThreadState)
            {
                case ThreadState.Running:
                case ThreadState.Suspended:
                case ThreadState.SuspendRequested:
                case ThreadState.WaitSleepJoin:
                    StopThread();
                    break;
            }
        }
    }
}
