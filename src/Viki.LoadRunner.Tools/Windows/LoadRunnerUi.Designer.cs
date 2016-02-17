﻿namespace Viki.LoadRunner.Tools.Windows
{
    partial class LoadRunnerUi
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
            this._startButton = new System.Windows.Forms.Button();
            this.resultsTextBox = new System.Windows.Forms.RichTextBox();
            this._backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this._stopButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _startButton
            // 
            this._startButton.Location = new System.Drawing.Point(537, 12);
            this._startButton.Name = "_startButton";
            this._startButton.Size = new System.Drawing.Size(75, 23);
            this._startButton.TabIndex = 0;
            this._startButton.Text = "Start";
            this._startButton.UseVisualStyleBackColor = true;
            this._startButton.Click += new System.EventHandler(this._startButton_Click);
            // 
            // resultsTextBox
            // 
            this.resultsTextBox.Location = new System.Drawing.Point(12, 12);
            this.resultsTextBox.Name = "resultsTextBox";
            this.resultsTextBox.Size = new System.Drawing.Size(519, 460);
            this.resultsTextBox.TabIndex = 2;
            this.resultsTextBox.Text = "";
            // 
            // _backgroundWorker1
            // 
            this._backgroundWorker1.WorkerSupportsCancellation = true;
            this._backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this._backgroundWorker1_DoWork);
            // 
            // _stopButton
            // 
            this._stopButton.Enabled = false;
            this._stopButton.Location = new System.Drawing.Point(537, 41);
            this._stopButton.Name = "_stopButton";
            this._stopButton.Size = new System.Drawing.Size(75, 23);
            this._stopButton.TabIndex = 3;
            this._stopButton.Text = "Stop";
            this._stopButton.UseVisualStyleBackColor = true;
            this._stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // LoadRunnerUi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 484);
            this.Controls.Add(this._stopButton);
            this.Controls.Add(this.resultsTextBox);
            this.Controls.Add(this._startButton);
            this.Name = "LoadRunnerUi";
            this.Text = "LoadRunnerUi";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _startButton;
        private System.Windows.Forms.RichTextBox resultsTextBox;
        private System.ComponentModel.BackgroundWorker _backgroundWorker1;
        private System.Windows.Forms.Button _stopButton;
    }
}