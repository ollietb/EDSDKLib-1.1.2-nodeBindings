namespace WinForms_Net45
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
            this.SaveToGroupBox = new System.Windows.Forms.GroupBox();
            this.ST_BothRdB = new System.Windows.Forms.RadioButton();
            this.ST_ComputerRdB = new System.Windows.Forms.RadioButton();
            this.ST_CameraRdB = new System.Windows.Forms.RadioButton();
            this.SavePathTextBox = new System.Windows.Forms.TextBox();
            this.BulbTextBox = new System.Windows.Forms.TextBox();
            this.WBCoBox = new System.Windows.Forms.ComboBox();
            this.ISOCoBox = new System.Windows.Forms.ComboBox();
            this.TvCoBox = new System.Windows.Forms.ComboBox();
            this.AvCoBox = new System.Windows.Forms.ComboBox();
            this.MainProgressBar = new System.Windows.Forms.ProgressBar();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.LVButton = new System.Windows.Forms.Button();
            this.RecordButton = new System.Windows.Forms.Button();
            this.PhotoButton = new System.Windows.Forms.Button();
            this.SessionButton = new System.Windows.Forms.Button();
            this.CameraSessionLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.CameraNameLabel = new System.Windows.Forms.Label();
            this.CameraListBox = new System.Windows.Forms.ListBox();
            this.RefreshButton = new System.Windows.Forms.Button();
            this.SaveFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.MainLiveView = new EOSDigital.Controls.WinForms.LiveView();
            this.SaveToGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // SaveToGroupBox
            // 
            this.SaveToGroupBox.Controls.Add(this.ST_BothRdB);
            this.SaveToGroupBox.Controls.Add(this.ST_ComputerRdB);
            this.SaveToGroupBox.Controls.Add(this.ST_CameraRdB);
            this.SaveToGroupBox.Location = new System.Drawing.Point(528, 10);
            this.SaveToGroupBox.Name = "SaveToGroupBox";
            this.SaveToGroupBox.Size = new System.Drawing.Size(84, 79);
            this.SaveToGroupBox.TabIndex = 32;
            this.SaveToGroupBox.TabStop = false;
            this.SaveToGroupBox.Text = "Save To";
            // 
            // ST_BothRdB
            // 
            this.ST_BothRdB.AutoSize = true;
            this.ST_BothRdB.Location = new System.Drawing.Point(6, 53);
            this.ST_BothRdB.Name = "ST_BothRdB";
            this.ST_BothRdB.Size = new System.Drawing.Size(47, 17);
            this.ST_BothRdB.TabIndex = 1;
            this.ST_BothRdB.Text = "Both";
            this.ST_BothRdB.UseVisualStyleBackColor = true;
            this.ST_BothRdB.CheckedChanged += new System.EventHandler(this.SaveTo_CheckedChanged);
            // 
            // ST_ComputerRdB
            // 
            this.ST_ComputerRdB.AutoSize = true;
            this.ST_ComputerRdB.Location = new System.Drawing.Point(6, 35);
            this.ST_ComputerRdB.Name = "ST_ComputerRdB";
            this.ST_ComputerRdB.Size = new System.Drawing.Size(70, 17);
            this.ST_ComputerRdB.TabIndex = 1;
            this.ST_ComputerRdB.Text = "Computer";
            this.ST_ComputerRdB.UseVisualStyleBackColor = true;
            this.ST_ComputerRdB.CheckedChanged += new System.EventHandler(this.SaveTo_CheckedChanged);
            // 
            // ST_CameraRdB
            // 
            this.ST_CameraRdB.AutoSize = true;
            this.ST_CameraRdB.Checked = true;
            this.ST_CameraRdB.Location = new System.Drawing.Point(6, 17);
            this.ST_CameraRdB.Name = "ST_CameraRdB";
            this.ST_CameraRdB.Size = new System.Drawing.Size(61, 17);
            this.ST_CameraRdB.TabIndex = 0;
            this.ST_CameraRdB.TabStop = true;
            this.ST_CameraRdB.Text = "Camera";
            this.ST_CameraRdB.UseVisualStyleBackColor = true;
            this.ST_CameraRdB.CheckedChanged += new System.EventHandler(this.SaveTo_CheckedChanged);
            // 
            // SavePathTextBox
            // 
            this.SavePathTextBox.Location = new System.Drawing.Point(140, 128);
            this.SavePathTextBox.Name = "SavePathTextBox";
            this.SavePathTextBox.Size = new System.Drawing.Size(391, 20);
            this.SavePathTextBox.TabIndex = 31;
            // 
            // BulbTextBox
            // 
            this.BulbTextBox.Location = new System.Drawing.Point(423, 38);
            this.BulbTextBox.Name = "BulbTextBox";
            this.BulbTextBox.Size = new System.Drawing.Size(75, 20);
            this.BulbTextBox.TabIndex = 30;
            this.BulbTextBox.Text = "10";
            this.BulbTextBox.TextChanged += new System.EventHandler(this.BulbTextBox_TextChanged);
            // 
            // WBCoBox
            // 
            this.WBCoBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.WBCoBox.FormattingEnabled = true;
            this.WBCoBox.Items.AddRange(new object[] {
            "Auto",
            "Daylight",
            "Cloudy",
            "Shade",
            "Tangsten",
            "Fluorescent",
            "Strobe"});
            this.WBCoBox.Location = new System.Drawing.Point(246, 94);
            this.WBCoBox.Name = "WBCoBox";
            this.WBCoBox.Size = new System.Drawing.Size(135, 21);
            this.WBCoBox.TabIndex = 29;
            this.WBCoBox.SelectedIndexChanged += new System.EventHandler(this.WBCoBox_SelectedIndexChanged);
            // 
            // ISOCoBox
            // 
            this.ISOCoBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ISOCoBox.FormattingEnabled = true;
            this.ISOCoBox.Location = new System.Drawing.Point(246, 66);
            this.ISOCoBox.Name = "ISOCoBox";
            this.ISOCoBox.Size = new System.Drawing.Size(135, 21);
            this.ISOCoBox.TabIndex = 28;
            this.ISOCoBox.SelectedIndexChanged += new System.EventHandler(this.ISOCoBox_SelectedIndexChanged);
            // 
            // TvCoBox
            // 
            this.TvCoBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TvCoBox.FormattingEnabled = true;
            this.TvCoBox.Location = new System.Drawing.Point(246, 39);
            this.TvCoBox.Name = "TvCoBox";
            this.TvCoBox.Size = new System.Drawing.Size(135, 21);
            this.TvCoBox.TabIndex = 27;
            this.TvCoBox.SelectedIndexChanged += new System.EventHandler(this.TvCoBox_SelectedIndexChanged);
            // 
            // AvCoBox
            // 
            this.AvCoBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AvCoBox.FormattingEnabled = true;
            this.AvCoBox.Location = new System.Drawing.Point(246, 12);
            this.AvCoBox.Name = "AvCoBox";
            this.AvCoBox.Size = new System.Drawing.Size(135, 21);
            this.AvCoBox.TabIndex = 26;
            this.AvCoBox.SelectedIndexChanged += new System.EventHandler(this.AvCoBox_SelectedIndexChanged);
            // 
            // MainProgressBar
            // 
            this.MainProgressBar.Location = new System.Drawing.Point(140, 94);
            this.MainProgressBar.Name = "MainProgressBar";
            this.MainProgressBar.Size = new System.Drawing.Size(100, 23);
            this.MainProgressBar.TabIndex = 25;
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(537, 125);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(75, 23);
            this.BrowseButton.TabIndex = 23;
            this.BrowseButton.Text = "Browse...";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // LVButton
            // 
            this.LVButton.Location = new System.Drawing.Point(423, 92);
            this.LVButton.Name = "LVButton";
            this.LVButton.Size = new System.Drawing.Size(96, 23);
            this.LVButton.TabIndex = 24;
            this.LVButton.Text = "Start Live View";
            this.LVButton.UseVisualStyleBackColor = true;
            this.LVButton.Click += new System.EventHandler(this.LVButton_Click);
            // 
            // RecordButton
            // 
            this.RecordButton.Location = new System.Drawing.Point(423, 63);
            this.RecordButton.Name = "RecordButton";
            this.RecordButton.Size = new System.Drawing.Size(96, 23);
            this.RecordButton.TabIndex = 22;
            this.RecordButton.Text = "Start Recording";
            this.RecordButton.UseVisualStyleBackColor = true;
            this.RecordButton.Click += new System.EventHandler(this.RecordButton_Click);
            // 
            // PhotoButton
            // 
            this.PhotoButton.Location = new System.Drawing.Point(423, 10);
            this.PhotoButton.Name = "PhotoButton";
            this.PhotoButton.Size = new System.Drawing.Size(99, 23);
            this.PhotoButton.TabIndex = 21;
            this.PhotoButton.Text = "Take Photo";
            this.PhotoButton.UseVisualStyleBackColor = true;
            this.PhotoButton.Click += new System.EventHandler(this.PhotoButton_Click);
            // 
            // SessionButton
            // 
            this.SessionButton.Location = new System.Drawing.Point(140, 66);
            this.SessionButton.Name = "SessionButton";
            this.SessionButton.Size = new System.Drawing.Size(100, 23);
            this.SessionButton.TabIndex = 20;
            this.SessionButton.Text = "Open Session";
            this.SessionButton.UseVisualStyleBackColor = true;
            this.SessionButton.Click += new System.EventHandler(this.SessionButton_Click);
            // 
            // CameraSessionLabel
            // 
            this.CameraSessionLabel.AutoSize = true;
            this.CameraSessionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CameraSessionLabel.Location = new System.Drawing.Point(138, 40);
            this.CameraSessionLabel.Name = "CameraSessionLabel";
            this.CameraSessionLabel.Size = new System.Drawing.Size(103, 16);
            this.CameraSessionLabel.TabIndex = 19;
            this.CameraSessionLabel.Text = "Session Closed";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(387, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 16);
            this.label6.TabIndex = 18;
            this.label6.Text = "WB";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(504, 40);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(15, 16);
            this.label7.TabIndex = 17;
            this.label7.Text = "s";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(387, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 16);
            this.label5.TabIndex = 16;
            this.label5.Text = "ISO";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(387, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 16);
            this.label4.TabIndex = 15;
            this.label4.Text = "Tv";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(387, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 16);
            this.label3.TabIndex = 14;
            this.label3.Text = "Av";
            // 
            // CameraNameLabel
            // 
            this.CameraNameLabel.AutoSize = true;
            this.CameraNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CameraNameLabel.Location = new System.Drawing.Point(138, 13);
            this.CameraNameLabel.Name = "CameraNameLabel";
            this.CameraNameLabel.Size = new System.Drawing.Size(77, 16);
            this.CameraNameLabel.TabIndex = 13;
            this.CameraNameLabel.Text = "No Camera";
            // 
            // CameraListBox
            // 
            this.CameraListBox.FormattingEnabled = true;
            this.CameraListBox.Location = new System.Drawing.Point(12, 12);
            this.CameraListBox.Name = "CameraListBox";
            this.CameraListBox.Size = new System.Drawing.Size(120, 108);
            this.CameraListBox.TabIndex = 12;
            this.CameraListBox.SelectedIndexChanged += new System.EventHandler(this.CameraListBox_SelectedIndexChanged);
            // 
            // RefreshButton
            // 
            this.RefreshButton.Location = new System.Drawing.Point(12, 126);
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(120, 23);
            this.RefreshButton.TabIndex = 11;
            this.RefreshButton.Text = "Refresh Cameras";
            this.RefreshButton.UseVisualStyleBackColor = true;
            this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // SaveFolderBrowser
            // 
            this.SaveFolderBrowser.RootFolder = System.Environment.SpecialFolder.MyPictures;
            // 
            // MainLiveView
            // 
            this.MainLiveView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainLiveView.BackColor = System.Drawing.SystemColors.ControlDark;
            this.MainLiveView.ImagePosition = EOSDigital.Controls.ImageAlign.Middle;
            this.MainLiveView.ImageTransform = EOSDigital.Controls.ImageTransformation.None;
            this.MainLiveView.Location = new System.Drawing.Point(12, 155);
            this.MainLiveView.MainCamera = null;
            this.MainLiveView.Name = "MainLiveView";
            this.MainLiveView.Size = new System.Drawing.Size(598, 399);
            this.MainLiveView.TabIndex = 33;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 566);
            this.Controls.Add(this.MainLiveView);
            this.Controls.Add(this.SaveToGroupBox);
            this.Controls.Add(this.SavePathTextBox);
            this.Controls.Add(this.BulbTextBox);
            this.Controls.Add(this.WBCoBox);
            this.Controls.Add(this.ISOCoBox);
            this.Controls.Add(this.TvCoBox);
            this.Controls.Add(this.AvCoBox);
            this.Controls.Add(this.MainProgressBar);
            this.Controls.Add(this.BrowseButton);
            this.Controls.Add(this.LVButton);
            this.Controls.Add(this.RecordButton);
            this.Controls.Add(this.PhotoButton);
            this.Controls.Add(this.SessionButton);
            this.Controls.Add(this.CameraSessionLabel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.CameraNameLabel);
            this.Controls.Add(this.CameraListBox);
            this.Controls.Add(this.RefreshButton);
            this.MinimumSize = new System.Drawing.Size(638, 604);
            this.Name = "MainForm";
            this.Text = "EDSDKAPI .Net4.5 Windows Forms Example";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.SaveToGroupBox.ResumeLayout(false);
            this.SaveToGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox SaveToGroupBox;
        private System.Windows.Forms.RadioButton ST_BothRdB;
        private System.Windows.Forms.RadioButton ST_ComputerRdB;
        private System.Windows.Forms.RadioButton ST_CameraRdB;
        private System.Windows.Forms.TextBox SavePathTextBox;
        private System.Windows.Forms.TextBox BulbTextBox;
        private System.Windows.Forms.ComboBox WBCoBox;
        private System.Windows.Forms.ComboBox ISOCoBox;
        private System.Windows.Forms.ComboBox TvCoBox;
        private System.Windows.Forms.ComboBox AvCoBox;
        private System.Windows.Forms.ProgressBar MainProgressBar;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Button LVButton;
        private System.Windows.Forms.Button RecordButton;
        private System.Windows.Forms.Button PhotoButton;
        private System.Windows.Forms.Button SessionButton;
        private System.Windows.Forms.Label CameraSessionLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label CameraNameLabel;
        private System.Windows.Forms.ListBox CameraListBox;
        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.FolderBrowserDialog SaveFolderBrowser;
        private EOSDigital.Controls.WinForms.LiveView MainLiveView;
    }
}

