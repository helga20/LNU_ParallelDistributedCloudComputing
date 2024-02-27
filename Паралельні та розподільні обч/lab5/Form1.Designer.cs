namespace lab5
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
            this.button1 = new System.Windows.Forms.Button();
            this.Result = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Thread1Status = new System.Windows.Forms.Label();
            this.Thread2Status = new System.Windows.Forms.Label();
            this.Thread3Status = new System.Windows.Forms.Label();
            this.Thread4Status = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(363, 388);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 29);
            this.button1.TabIndex = 4;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Result
            // 
            this.Result.Location = new System.Drawing.Point(569, 76);
            this.Result.Multiline = true;
            this.Result.Name = "Result";
            this.Result.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Result.Size = new System.Drawing.Size(145, 212);
            this.Result.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(601, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "Result:";
            // 
            // Thread1Status
            // 
            this.Thread1Status.AutoSize = true;
            this.Thread1Status.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Thread1Status.ForeColor = System.Drawing.Color.Red;
            this.Thread1Status.Location = new System.Drawing.Point(363, 48);
            this.Thread1Status.Name = "Thread1Status";
            this.Thread1Status.Size = new System.Drawing.Size(100, 35);
            this.Thread1Status.TabIndex = 11;
            this.Thread1Status.Text = "Inactive";
            // 
            // Thread2Status
            // 
            this.Thread2Status.AutoSize = true;
            this.Thread2Status.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Thread2Status.ForeColor = System.Drawing.Color.Red;
            this.Thread2Status.Location = new System.Drawing.Point(363, 112);
            this.Thread2Status.Name = "Thread2Status";
            this.Thread2Status.Size = new System.Drawing.Size(100, 35);
            this.Thread2Status.TabIndex = 12;
            this.Thread2Status.Text = "Inactive";
            // 
            // Thread3Status
            // 
            this.Thread3Status.AutoSize = true;
            this.Thread3Status.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Thread3Status.ForeColor = System.Drawing.Color.Red;
            this.Thread3Status.Location = new System.Drawing.Point(363, 182);
            this.Thread3Status.Name = "Thread3Status";
            this.Thread3Status.Size = new System.Drawing.Size(100, 35);
            this.Thread3Status.TabIndex = 13;
            this.Thread3Status.Text = "Inactive";
            // 
            // Thread4Status
            // 
            this.Thread4Status.AutoSize = true;
            this.Thread4Status.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Thread4Status.ForeColor = System.Drawing.Color.Red;
            this.Thread4Status.Location = new System.Drawing.Point(363, 253);
            this.Thread4Status.Name = "Thread4Status";
            this.Thread4Status.Size = new System.Drawing.Size(100, 35);
            this.Thread4Status.TabIndex = 14;
            this.Thread4Status.Text = "Inactive";
            // 
            // label2
            // 
            this.label2.AllowDrop = true;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(167, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 25);
            this.label2.TabIndex = 15;
            this.label2.Text = "Thread 1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(167, 258);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 30);
            this.label4.TabIndex = 17;
            this.label4.Text = "Thread 4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(167, 187);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 30);
            this.label5.TabIndex = 18;
            this.label5.Text = "Thread 3";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(167, 117);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 30);
            this.label6.TabIndex = 19;
            this.label6.Text = "Thread 2";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Thread4Status);
            this.Controls.Add(this.Thread3Status);
            this.Controls.Add(this.Thread2Status);
            this.Controls.Add(this.Thread1Status);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Result);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox Result;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;

        private System.Windows.Forms.Label Thread1Status;
        private System.Windows.Forms.Label Thread2Status;
        private System.Windows.Forms.Label Thread3Status;
        private System.Windows.Forms.Label Thread4Status;
       
        
        
    }
}
