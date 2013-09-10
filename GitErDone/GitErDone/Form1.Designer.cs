namespace GitErDone
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlMostProductiveSongs = new System.Windows.Forms.Panel();
            this.pnlMostProductiveHrs = new System.Windows.Forms.Panel();
            this.lblStats = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(115, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(563, 38);
            this.label1.TabIndex = 0;
            this.label1.Text = "CodeChord: the productive you analysis tool";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Most productive songs";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(376, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(148, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Most productive hours";
            // 
            // pnlMostProductiveSongs
            // 
            this.pnlMostProductiveSongs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMostProductiveSongs.Location = new System.Drawing.Point(15, 76);
            this.pnlMostProductiveSongs.Name = "pnlMostProductiveSongs";
            this.pnlMostProductiveSongs.Size = new System.Drawing.Size(358, 310);
            this.pnlMostProductiveSongs.TabIndex = 3;
            // 
            // pnlMostProductiveHrs
            // 
            this.pnlMostProductiveHrs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMostProductiveHrs.Location = new System.Drawing.Point(379, 76);
            this.pnlMostProductiveHrs.Name = "pnlMostProductiveHrs";
            this.pnlMostProductiveHrs.Size = new System.Drawing.Size(358, 310);
            this.pnlMostProductiveHrs.TabIndex = 5;
            // 
            // lblStats
            // 
            this.lblStats.AutoSize = true;
            this.lblStats.Location = new System.Drawing.Point(15, 393);
            this.lblStats.Name = "lblStats";
            this.lblStats.Size = new System.Drawing.Size(134, 85);
            this.lblStats.TabIndex = 6;
            this.lblStats.Text = "Session statistics\r\nSongs listened to: X\r\nSession length: Y\r\nX\r\nY";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(758, 484);
            this.Controls.Add(this.lblStats);
            this.Controls.Add(this.pnlMostProductiveHrs);
            this.Controls.Add(this.pnlMostProductiveSongs);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.Text = "CodeChord";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel pnlMostProductiveSongs;
        private System.Windows.Forms.Panel pnlMostProductiveHrs;
        private System.Windows.Forms.Label lblStats;

    }
}

