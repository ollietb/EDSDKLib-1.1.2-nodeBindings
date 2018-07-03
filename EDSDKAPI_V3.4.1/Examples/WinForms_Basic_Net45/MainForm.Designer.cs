namespace WinForms_Basic_Net45
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
            this.SaveToComputerChBox = new System.Windows.Forms.CheckBox();
            this.SavePathTextBox = new System.Windows.Forms.TextBox();
            this.PhotoButton = new System.Windows.Forms.Button();
            this.CameraLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SaveToComputerChBox
            // 
            this.SaveToComputerChBox.AutoSize = true;
            this.SaveToComputerChBox.Location = new System.Drawing.Point(111, 70);
            this.SaveToComputerChBox.Margin = new System.Windows.Forms.Padding(4);
            this.SaveToComputerChBox.Name = "SaveToComputerChBox";
            this.SaveToComputerChBox.Size = new System.Drawing.Size(181, 22);
            this.SaveToComputerChBox.TabIndex = 7;
            this.SaveToComputerChBox.Text = "Download to Computer";
            this.SaveToComputerChBox.UseVisualStyleBackColor = true;
            this.SaveToComputerChBox.CheckedChanged += new System.EventHandler(this.SaveToComputerChBox_CheckedChanged);
            // 
            // SavePathTextBox
            // 
            this.SavePathTextBox.Location = new System.Drawing.Point(111, 38);
            this.SavePathTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.SavePathTextBox.Name = "SavePathTextBox";
            this.SavePathTextBox.Size = new System.Drawing.Size(308, 24);
            this.SavePathTextBox.TabIndex = 6;
            // 
            // PhotoButton
            // 
            this.PhotoButton.Location = new System.Drawing.Point(13, 34);
            this.PhotoButton.Margin = new System.Windows.Forms.Padding(4);
            this.PhotoButton.Name = "PhotoButton";
            this.PhotoButton.Size = new System.Drawing.Size(90, 61);
            this.PhotoButton.TabIndex = 5;
            this.PhotoButton.Text = "Take Photo";
            this.PhotoButton.UseVisualStyleBackColor = true;
            this.PhotoButton.Click += new System.EventHandler(this.PhotoButton_Click);
            // 
            // CameraLabel
            // 
            this.CameraLabel.AutoSize = true;
            this.CameraLabel.Location = new System.Drawing.Point(13, 9);
            this.CameraLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CameraLabel.Name = "CameraLabel";
            this.CameraLabel.Size = new System.Drawing.Size(155, 18);
            this.CameraLabel.TabIndex = 4;
            this.CameraLabel.Text = "No camera connected";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 108);
            this.Controls.Add(this.SaveToComputerChBox);
            this.Controls.Add(this.SavePathTextBox);
            this.Controls.Add(this.PhotoButton);
            this.Controls.Add(this.CameraLabel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "EDSDKAPI .Net4.5 Basic Windows Forms Example";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox SaveToComputerChBox;
        private System.Windows.Forms.TextBox SavePathTextBox;
        private System.Windows.Forms.Button PhotoButton;
        private System.Windows.Forms.Label CameraLabel;


    }
}

