
namespace ShortestPathGenetic
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.currentPathLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.bestPathPanel = new System.Windows.Forms.Panel();
            this.CurrentBestPathLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.bestPathPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.currentPathLabel);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.MaximumSize = new System.Drawing.Size(450, 560);
            this.panel1.MinimumSize = new System.Drawing.Size(450, 560);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(450, 560);
            this.panel1.TabIndex = 0;
            // 
            // currentPathLabel
            // 
            this.currentPathLabel.AutoSize = true;
            this.currentPathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentPathLabel.Location = new System.Drawing.Point(218, 18);
            this.currentPathLabel.Name = "currentPathLabel";
            this.currentPathLabel.Size = new System.Drawing.Size(107, 20);
            this.currentPathLabel.TabIndex = 1;
            this.currentPathLabel.Text = "Current Path :";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Window;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_ClickAsync);
            // 
            // bestPathPanel
            // 
            this.bestPathPanel.Controls.Add(this.CurrentBestPathLabel);
            this.bestPathPanel.Location = new System.Drawing.Point(450, 0);
            this.bestPathPanel.MaximumSize = new System.Drawing.Size(450, 560);
            this.bestPathPanel.MinimumSize = new System.Drawing.Size(450, 560);
            this.bestPathPanel.Name = "bestPathPanel";
            this.bestPathPanel.Size = new System.Drawing.Size(450, 560);
            this.bestPathPanel.TabIndex = 1;
            // 
            // CurrentBestPathLabel
            // 
            this.CurrentBestPathLabel.AutoSize = true;
            this.CurrentBestPathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentBestPathLabel.Location = new System.Drawing.Point(175, 18);
            this.CurrentBestPathLabel.Name = "CurrentBestPathLabel";
            this.CurrentBestPathLabel.Size = new System.Drawing.Size(144, 20);
            this.CurrentBestPathLabel.TabIndex = 3;
            this.CurrentBestPathLabel.Text = "Current Best Path :";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 557);
            this.Controls.Add(this.bestPathPanel);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.bestPathPanel.ResumeLayout(false);
            this.bestPathPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel bestPathPanel;
        private System.Windows.Forms.Label currentPathLabel;
        private System.Windows.Forms.Label CurrentBestPathLabel;
    }
}

