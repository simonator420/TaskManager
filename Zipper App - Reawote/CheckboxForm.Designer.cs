namespace Zipper_App___Reawote
{
    partial class CheckboxForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckboxForm));
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.prevedDoJPG = new System.Windows.Forms.Button();
            this.selectAllButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.BackColor = System.Drawing.SystemColors.Window;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(12, 45);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(366, 349);
            this.checkedListBox1.TabIndex = 32;
            this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
            // 
            // prevedDoJPG
            // 
            this.prevedDoJPG.BackColor = System.Drawing.Color.Black;
            this.prevedDoJPG.Font = new System.Drawing.Font("Microsoft PhagsPa", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prevedDoJPG.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.prevedDoJPG.Location = new System.Drawing.Point(94, 435);
            this.prevedDoJPG.Name = "prevedDoJPG";
            this.prevedDoJPG.Size = new System.Drawing.Size(187, 38);
            this.prevedDoJPG.TabIndex = 33;
            this.prevedDoJPG.Text = "PŘEVEĎ DO .JPG";
            this.prevedDoJPG.UseVisualStyleBackColor = false;
            this.prevedDoJPG.Click += new System.EventHandler(this.prevedDoJPG_Click);
            // 
            // selectAllButton
            // 
            this.selectAllButton.BackColor = System.Drawing.Color.Black;
            this.selectAllButton.Font = new System.Drawing.Font("Microsoft PhagsPa", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectAllButton.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.selectAllButton.Location = new System.Drawing.Point(12, 400);
            this.selectAllButton.Name = "selectAllButton";
            this.selectAllButton.Size = new System.Drawing.Size(76, 30);
            this.selectAllButton.TabIndex = 34;
            this.selectAllButton.Text = "Select All";
            this.selectAllButton.UseVisualStyleBackColor = false;
            this.selectAllButton.Click += new System.EventHandler(this.selectAllButton_Click);
            // 
            // CheckboxForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(390, 496);
            this.Controls.Add(this.selectAllButton);
            this.Controls.Add(this.prevedDoJPG);
            this.Controls.Add(this.checkedListBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "CheckboxForm";
            this.Text = "Vyber Checkboxy";
            this.Load += new System.EventHandler(this.CheckboxForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Button prevedDoJPG;
        private System.Windows.Forms.Button selectAllButton;
    }
}