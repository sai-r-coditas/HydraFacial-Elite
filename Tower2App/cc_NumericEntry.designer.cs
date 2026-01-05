namespace Edge.Tower2.UI
{
    partial class cc_NumericEntry
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
            this.pictureBoxNumericEntry = new System.Windows.Forms.PictureBox();
            this.textBoxEntry = new System.Windows.Forms.TextBox();
            this.textBoxAccountNumber = new System.Windows.Forms.TextBox();
            this.Next = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btnHide = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNumericEntry)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxNumericEntry
            // 
            this.pictureBoxNumericEntry.BackColor = System.Drawing.Color.White;
            this.pictureBoxNumericEntry.Location = new System.Drawing.Point(194, 161);
            this.pictureBoxNumericEntry.Name = "pictureBoxNumericEntry";
            this.pictureBoxNumericEntry.Size = new System.Drawing.Size(922, 633);
            this.pictureBoxNumericEntry.TabIndex = 17;
            this.pictureBoxNumericEntry.TabStop = false;
            this.pictureBoxNumericEntry.Click += new System.EventHandler(this.pictureBoxNumericEntry_Click);
            this.pictureBoxNumericEntry.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxNumericEntry_MouseUp);
            // 
            // textBoxEntry
            // 
            this.textBoxEntry.Font = new System.Drawing.Font("Tahoma", 18F);
            this.textBoxEntry.Location = new System.Drawing.Point(424, 247);
            this.textBoxEntry.Name = "textBoxEntry";
            this.textBoxEntry.Size = new System.Drawing.Size(403, 36);
            this.textBoxEntry.TabIndex = 14;
            // 
            // textBoxAccountNumber
            // 
            this.textBoxAccountNumber.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.textBoxAccountNumber.Font = new System.Drawing.Font("Tahoma", 12F);
            this.textBoxAccountNumber.Location = new System.Drawing.Point(471, 216);
            this.textBoxAccountNumber.Name = "textBoxAccountNumber";
            this.textBoxAccountNumber.ReadOnly = true;
            this.textBoxAccountNumber.Size = new System.Drawing.Size(161, 27);
            this.textBoxAccountNumber.TabIndex = 16;
            this.textBoxAccountNumber.Visible = false;
            // 
            // Next
            // 
            this.Next.Location = new System.Drawing.Point(1139, 110);
            this.Next.Name = "Next";
            this.Next.Size = new System.Drawing.Size(75, 60);
            this.Next.TabIndex = 18;
            this.Next.Text = "Next";
            this.Next.UseVisualStyleBackColor = true;
            this.Next.Visible = false;
            this.Next.Click += new System.EventHandler(this.button1_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1139, 208);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 60);
            this.button1.TabIndex = 19;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1140, 305);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 60);
            this.button2.TabIndex = 20;
            this.button2.Text = "TowerII";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnHide
            // 
            this.btnHide.Location = new System.Drawing.Point(1143, 389);
            this.btnHide.Name = "btnHide";
            this.btnHide.Size = new System.Drawing.Size(75, 53);
            this.btnHide.TabIndex = 21;
            this.btnHide.Text = "Hide";
            this.btnHide.UseVisualStyleBackColor = true;
            this.btnHide.Click += new System.EventHandler(this.btnHide_Click);
            // 
            // cc_NumericEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1280, 1024);
            this.Controls.Add(this.btnHide);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Next);
            this.Controls.Add(this.textBoxAccountNumber);
            this.Controls.Add(this.textBoxEntry);
            this.Controls.Add(this.pictureBoxNumericEntry);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "cc_NumericEntry";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.NumericEntry_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNumericEntry)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxNumericEntry;
        private System.Windows.Forms.TextBox textBoxEntry;
        private System.Windows.Forms.TextBox textBoxAccountNumber;
        private System.Windows.Forms.Button Next;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnHide;
    }
}

