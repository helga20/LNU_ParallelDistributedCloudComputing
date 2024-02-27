namespace Forms
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.stop1 = new System.Windows.Forms.Button();
            this.continue1 = new System.Windows.Forms.Button();
            this.ballPanel = new System.Windows.Forms.Panel();
            this.sinusPanel = new System.Windows.Forms.Panel();
            this.rectanglePanel = new System.Windows.Forms.Panel();
            this.circlePanel = new System.Windows.Forms.Panel();
            this.stop2 = new System.Windows.Forms.Button();
            this.continue2 = new System.Windows.Forms.Button();
            this.stop3 = new System.Windows.Forms.Button();
            this.continue3 = new System.Windows.Forms.Button();
            this.stop4 = new System.Windows.Forms.Button();
            this.continue4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // stop1
            // 
            this.stop1.Location = new System.Drawing.Point(186, 169);
            this.stop1.Name = "stop1";
            this.stop1.Size = new System.Drawing.Size(91, 29);
            this.stop1.TabIndex = 0;
            this.stop1.Text = "stop1";
            this.stop1.UseVisualStyleBackColor = true;
            this.stop1.Click += new System.EventHandler(this.stop1_Click);
            // 
            // continue1
            // 
            this.continue1.Location = new System.Drawing.Point(73, 169);
            this.continue1.Name = "continue1";
            this.continue1.Size = new System.Drawing.Size(91, 29);
            this.continue1.TabIndex = 1;
            this.continue1.Text = "continue1";
            this.continue1.UseVisualStyleBackColor = true;
            this.continue1.Click += new System.EventHandler(this.continue1_Click);
            // 
            // ballPanel
            // 
            this.ballPanel.Location = new System.Drawing.Point(73, 36);
            this.ballPanel.Name = "ballPanel";
            this.ballPanel.Size = new System.Drawing.Size(202, 114);
            this.ballPanel.TabIndex = 1;
            this.ballPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.ballPanel_Paint);
            // 
            // sinusPanel
            // 
            this.sinusPanel.Location = new System.Drawing.Point(472, 36);
            this.sinusPanel.Name = "sinusPanel";
            this.sinusPanel.Size = new System.Drawing.Size(202, 114);
            this.sinusPanel.TabIndex = 2;
            this.sinusPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.sinusPanel_Paint);
            // 
            // rectanglePanel
            // 
            this.rectanglePanel.Location = new System.Drawing.Point(73, 220);
            this.rectanglePanel.Name = "rectanglePanel";
            this.rectanglePanel.Size = new System.Drawing.Size(202, 111);
            this.rectanglePanel.TabIndex = 3;
            this.rectanglePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.rectanglePanel_Paint);
            // 
            // circlePanel
            // 
            this.circlePanel.Location = new System.Drawing.Point(472, 220);
            this.circlePanel.Name = "circlePanel";
            this.circlePanel.Size = new System.Drawing.Size(202, 111);
            this.circlePanel.TabIndex = 4;
            this.circlePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.clockPanel_Paint);
            // 
            // stop2
            // 
            this.stop2.Location = new System.Drawing.Point(186, 362);
            this.stop2.Name = "stop2";
            this.stop2.Size = new System.Drawing.Size(91, 29);
            this.stop2.TabIndex = 5;
            this.stop2.Text = "stop2";
            this.stop2.UseVisualStyleBackColor = true;
            this.stop2.Click += new System.EventHandler(this.stop2_Click);
            // 
            // continue2
            // 
            this.continue2.Location = new System.Drawing.Point(73, 362);
            this.continue2.Name = "continue2";
            this.continue2.Size = new System.Drawing.Size(91, 29);
            this.continue2.TabIndex = 4;
            this.continue2.Text = "continue2";
            this.continue2.UseVisualStyleBackColor = true;
            this.continue2.Click += new System.EventHandler(this.continue2_Click);
            // 
            // stop3
            // 
            this.stop3.Location = new System.Drawing.Point(583, 169);
            this.stop3.Name = "stop3";
            this.stop3.Size = new System.Drawing.Size(91, 29);
            this.stop3.TabIndex = 7;
            this.stop3.Text = "stop3";
            this.stop3.UseVisualStyleBackColor = true;
            this.stop3.Click += new System.EventHandler(this.stop3_Click);
            // 
            // continue3
            // 
            this.continue3.Location = new System.Drawing.Point(470, 169);
            this.continue3.Name = "continue3";
            this.continue3.Size = new System.Drawing.Size(91, 29);
            this.continue3.TabIndex = 6;
            this.continue3.Text = "continue3";
            this.continue3.UseVisualStyleBackColor = true;
            this.continue3.Click += new System.EventHandler(this.continue3_Click);
            // 
            // stop4
            // 
            this.stop4.Location = new System.Drawing.Point(585, 362);
            this.stop4.Name = "stop4";
            this.stop4.Size = new System.Drawing.Size(91, 29);
            this.stop4.TabIndex = 9;
            this.stop4.Text = "stop4";
            this.stop4.UseVisualStyleBackColor = true;
            this.stop4.Click += new System.EventHandler(this.stop4_Click);
            // 
            // continue4
            // 
            this.continue4.Location = new System.Drawing.Point(472, 362);
            this.continue4.Name = "continue4";
            this.continue4.Size = new System.Drawing.Size(91, 29);
            this.continue4.TabIndex = 8;
            this.continue4.Text = "continue4";
            this.continue4.UseVisualStyleBackColor = true;
            this.continue4.Click += new System.EventHandler(this.continue4_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.stop4);
            this.Controls.Add(this.continue4);
            this.Controls.Add(this.stop3);
            this.Controls.Add(this.continue3);
            this.Controls.Add(this.stop2);
            this.Controls.Add(this.continue2);
            this.Controls.Add(this.circlePanel);
            this.Controls.Add(this.rectanglePanel);
            this.Controls.Add(this.sinusPanel);
            this.Controls.Add(this.ballPanel);
            this.Controls.Add(this.continue1);
            this.Controls.Add(this.stop1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Button stop1;
        private Button continue1;
        private Panel ballPanel;
        private Panel sinusPanel;
        private Panel rectanglePanel;
        private Panel circlePanel;
        private Button stop2;
        private Button continue2;
        private Button stop3;
        private Button continue3;
        private Button stop4;
        private Button continue4;
    }
}