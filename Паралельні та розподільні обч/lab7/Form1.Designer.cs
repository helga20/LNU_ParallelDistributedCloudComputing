namespace lab7
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
            this.ThreadsActivity = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ManyThreads = new System.Windows.Forms.Button();
            this.MainThread = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ThreadsActivity
            // 
            this.ThreadsActivity.Location = new System.Drawing.Point(187, 65);
            this.ThreadsActivity.Multiline = true;
            this.ThreadsActivity.Name = "ThreadsActivity";
            this.ThreadsActivity.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ThreadsActivity.Size = new System.Drawing.Size(402, 250);
            this.ThreadsActivity.TabIndex = 1;
            this.ThreadsActivity.TextChanged += new System.EventHandler(this.ThreadsActivity_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(259, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(255, 33);
            this.label1.TabIndex = 2;
            this.label1.Text = "Threads Activity";
            // 
            // ManyThreads
            // 
            this.ManyThreads.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ManyThreads.Location = new System.Drawing.Point(466, 357);
            this.ManyThreads.Name = "ManyThreads";
            this.ManyThreads.Size = new System.Drawing.Size(123, 59);
            this.ManyThreads.TabIndex = 3;
            this.ManyThreads.Text = "Threads";
            this.ManyThreads.UseVisualStyleBackColor = true;
            this.ManyThreads.Click += new System.EventHandler(this.ManyThreads_Click);
            // 
            // MainThread
            // 
            this.MainThread.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.MainThread.Location = new System.Drawing.Point(187, 357);
            this.MainThread.Name = "MainThread";
            this.MainThread.Size = new System.Drawing.Size(128, 59);
            this.MainThread.TabIndex = 4;
            this.MainThread.Text = "Main Thread";
            this.MainThread.UseVisualStyleBackColor = true;
            this.MainThread.Click += new System.EventHandler(this.MainThread_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.MainThread);
            this.Controls.Add(this.ManyThreads);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ThreadsActivity);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public TextBox ThreadsActivity;
        private Label label1;
        private Button ManyThreads;
        private Button MainThread;
    }
}