namespace lab8
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
            this.label1 = new System.Windows.Forms.Label();
            this.ThreadsActivity = new System.Windows.Forms.TextBox();
            this.Start = new System.Windows.Forms.Button();
            this.ResultBox = new System.Windows.Forms.TextBox();
            this.ResultLabel = new System.Windows.Forms.Label();
            this.MainButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(44, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(271, 34);
            this.label1.TabIndex = 0;
            this.label1.Text = "Threads Activity";
            // 
            // ThreadsActivity
            // 
            this.ThreadsActivity.Font = new System.Drawing.Font("Consolas", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ThreadsActivity.Location = new System.Drawing.Point(12, 95);
            this.ThreadsActivity.Multiline = true;
            this.ThreadsActivity.Name = "ThreadsActivity";
            this.ThreadsActivity.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ThreadsActivity.Size = new System.Drawing.Size(348, 287);
            this.ThreadsActivity.TabIndex = 1;
            // 
            // Start
            // 
            this.Start.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Start.Location = new System.Drawing.Point(536, 409);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(129, 43);
            this.Start.TabIndex = 2;
            this.Start.Text = "Threads";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // ResultBox
            // 
            this.ResultBox.Font = new System.Drawing.Font("Consolas", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ResultBox.Location = new System.Drawing.Point(390, 95);
            this.ResultBox.Multiline = true;
            this.ResultBox.Name = "ResultBox";
            this.ResultBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ResultBox.Size = new System.Drawing.Size(392, 277);
            this.ResultBox.TabIndex = 3;
            // 
            // ResultLabel
            // 
            this.ResultLabel.AutoSize = true;
            this.ResultLabel.Font = new System.Drawing.Font("Consolas", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ResultLabel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ResultLabel.Location = new System.Drawing.Point(536, 49);
            this.ResultLabel.Name = "ResultLabel";
            this.ResultLabel.Size = new System.Drawing.Size(111, 34);
            this.ResultLabel.TabIndex = 4;
            this.ResultLabel.Text = "Result";
            // 
            // MainButton
            // 
            this.MainButton.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.MainButton.Location = new System.Drawing.Point(73, 409);
            this.MainButton.Name = "MainButton";
            this.MainButton.Size = new System.Drawing.Size(151, 43);
            this.MainButton.TabIndex = 5;
            this.MainButton.Text = "MainThread";
            this.MainButton.UseVisualStyleBackColor = true;
            this.MainButton.Click += new System.EventHandler(this.MainButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 478);
            this.Controls.Add(this.MainButton);
            this.Controls.Add(this.ResultLabel);
            this.Controls.Add(this.ResultBox);
            this.Controls.Add(this.Start);
            this.Controls.Add(this.ThreadsActivity);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TextBox ThreadsActivity;
        private Button Start;
        private TextBox ResultBox;
        private Label ResultLabel;
        private Button MainButton;
    }
}