﻿namespace GlobalMacroRecorder
{
    partial class MacroForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MacroForm));
            this.recordStartButton = new System.Windows.Forms.Button();
            this.recordStopButton = new System.Windows.Forms.Button();
            this.playBackMacroButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ScrollPanel = new System.Windows.Forms.Panel();
            this.speechButton = new System.Windows.Forms.Button();
            this.ImportSeveralEventsButton = new System.Windows.Forms.Button();
            this.ExportSeveralEventsButton = new System.Windows.Forms.Button();
            this.EventSettingButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // recordStartButton
            // 
            this.recordStartButton.Location = new System.Drawing.Point(122, 12);
            this.recordStartButton.Name = "recordStartButton";
            this.recordStartButton.Size = new System.Drawing.Size(75, 52);
            this.recordStartButton.TabIndex = 0;
            this.recordStartButton.Text = "Start";
            this.recordStartButton.UseVisualStyleBackColor = true;
            this.recordStartButton.Click += new System.EventHandler(this.recordStartButton_Click);
            // 
            // recordStopButton
            // 
            this.recordStopButton.Location = new System.Drawing.Point(203, 12);
            this.recordStopButton.Name = "recordStopButton";
            this.recordStopButton.Size = new System.Drawing.Size(75, 52);
            this.recordStopButton.TabIndex = 0;
            this.recordStopButton.Text = "Stop";
            this.recordStopButton.UseVisualStyleBackColor = true;
            this.recordStopButton.Click += new System.EventHandler(this.recordStopButton_Click);
            // 
            // playBackMacroButton
            // 
            this.playBackMacroButton.Location = new System.Drawing.Point(122, 70);
            this.playBackMacroButton.Name = "playBackMacroButton";
            this.playBackMacroButton.Size = new System.Drawing.Size(218, 42);
            this.playBackMacroButton.TabIndex = 1;
            this.playBackMacroButton.Text = "Play Back";
            this.playBackMacroButton.UseVisualStyleBackColor = true;
            this.playBackMacroButton.Click += new System.EventHandler(this.playBackMacroButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Record Events";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Playback Events";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ScrollPanel);
            this.groupBox1.Location = new System.Drawing.Point(10, 160);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(356, 177);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Event ID";
            // 
            // ScrollPanel
            // 
            this.ScrollPanel.AutoScroll = true;
            this.ScrollPanel.Location = new System.Drawing.Point(6, 12);
            this.ScrollPanel.Name = "ScrollPanel";
            this.ScrollPanel.Size = new System.Drawing.Size(344, 159);
            this.ScrollPanel.TabIndex = 0;
            // 
            // speechButton
            // 
            this.speechButton.Image = global::GlobalMacroRecorder.Properties.Resources.FreeNoSpeechRecognitionIcon;
            this.speechButton.Location = new System.Drawing.Point(284, 12);
            this.speechButton.Name = "speechButton";
            this.speechButton.Size = new System.Drawing.Size(56, 52);
            this.speechButton.TabIndex = 4;
            this.speechButton.UseVisualStyleBackColor = true;
            this.speechButton.Click += new System.EventHandler(this.speechButton_Click);
            // 
            // ImportSeveralEventsButton
            // 
            this.ImportSeveralEventsButton.Location = new System.Drawing.Point(19, 118);
            this.ImportSeveralEventsButton.Name = "ImportSeveralEventsButton";
            this.ImportSeveralEventsButton.Size = new System.Drawing.Size(120, 36);
            this.ImportSeveralEventsButton.TabIndex = 5;
            this.ImportSeveralEventsButton.Text = "Import Several Events";
            this.ImportSeveralEventsButton.UseVisualStyleBackColor = true;
            this.ImportSeveralEventsButton.Click += new System.EventHandler(this.ImportSeveralEventsButton_Click);
            // 
            // ExportSeveralEventsButton
            // 
            this.ExportSeveralEventsButton.Location = new System.Drawing.Point(146, 119);
            this.ExportSeveralEventsButton.Name = "ExportSeveralEventsButton";
            this.ExportSeveralEventsButton.Size = new System.Drawing.Size(120, 36);
            this.ExportSeveralEventsButton.TabIndex = 6;
            this.ExportSeveralEventsButton.Text = "Export Several Events";
            this.ExportSeveralEventsButton.UseVisualStyleBackColor = true;
            this.ExportSeveralEventsButton.Click += new System.EventHandler(this.ExportSeveralEventsButton_Click);
            // 
            // EventSettingButton
            // 
            this.EventSettingButton.Location = new System.Drawing.Point(274, 118);
            this.EventSettingButton.Name = "EventSettingButton";
            this.EventSettingButton.Size = new System.Drawing.Size(92, 36);
            this.EventSettingButton.TabIndex = 7;
            this.EventSettingButton.Text = "Event Setting";
            this.EventSettingButton.UseVisualStyleBackColor = true;
            this.EventSettingButton.Click += new System.EventHandler(this.EventSettingButton_Click);
            // 
            // MacroForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 346);
            this.Controls.Add(this.EventSettingButton);
            this.Controls.Add(this.ExportSeveralEventsButton);
            this.Controls.Add(this.ImportSeveralEventsButton);
            this.Controls.Add(this.speechButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.playBackMacroButton);
            this.Controls.Add(this.recordStopButton);
            this.Controls.Add(this.recordStartButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MacroForm";
            this.Text = "Iterum";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button recordStartButton;
        private System.Windows.Forms.Button recordStopButton;
        private System.Windows.Forms.Button playBackMacroButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel ScrollPanel;
        private System.Windows.Forms.Button speechButton;
        private System.Windows.Forms.Button ImportSeveralEventsButton;
        private System.Windows.Forms.Button ExportSeveralEventsButton;
        private System.Windows.Forms.Button EventSettingButton;
    }
}

