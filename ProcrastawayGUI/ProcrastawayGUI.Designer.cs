namespace ProcrastawayGUI
{
    partial class ProcrastawayGUI
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
            this.configBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.timeReport = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // configBox
            // 
            this.configBox.Location = new System.Drawing.Point(12, 41);
            this.configBox.Name = "configBox";
            this.configBox.Size = new System.Drawing.Size(275, 200);
            this.configBox.TabIndex = 1;
            this.configBox.TabStop = false;
            this.configBox.Text = "Configuration";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Remaining Playtime:";
            // 
            // timeReport
            // 
            this.timeReport.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.timeReport.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.timeReport.Location = new System.Drawing.Point(117, 8);
            this.timeReport.Name = "timeReport";
            this.timeReport.Size = new System.Drawing.Size(167, 20);
            this.timeReport.TabIndex = 3;
            this.timeReport.Text = "label2";
            // 
            // ProcrastawayGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 255);
            this.Controls.Add(this.timeReport);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.configBox);
            this.Name = "ProcrastawayGUI";
            this.Text = "Procrastaway Configurator";
            this.Load += new System.EventHandler(this.ProcrastawayGUI_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProcrastawayGUI_Close);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox configBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label timeReport;
    }
}

