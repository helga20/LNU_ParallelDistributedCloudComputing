using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;


namespace lab5
{
    public partial class Form1 : Form
    {
        private Label[] labels = new Label[4];
        private TextBox result = new TextBox();

        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)

        {
            Stopwatch sw = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            
            int V = 5;


            sw2.Start();
            // матриця суміжності 
            int[,] G = {
                { 0, 9, 75, 0, 0 },
                { 9, 0, 95, 19, 42},
                { 75, 95, 0, 51, 66},
                { 0, 19, 51, 0, 31},
                { 0, 42, 66, 31, 0},};

            labels[0] = Thread1Status;
            labels[1] = Thread2Status;
            labels[2] = Thread3Status;
            labels[3] = Thread4Status;


            
            Label l = new Label();
            PGraph p = new PGraph(G, V, l);
            p.Prim();
            
            string s = "";
            int k = 0;
            sw2.Stop();


            while (k <3)
            {
                
                List<PGraph> pr = new List<PGraph>();
                for (int i = 0; i < 4; i++)
                {
                    
                    pr.Add(new PGraph(G, V, labels[i]));
                    
                }
                
                sw.Start();
                Parallel.ForEach(pr, x => s = x.Prim());
                sw.Stop();
                

                k++;
            }


            

            Result.Text = s;
            result.AppendText($"\r\nTime for Threads: {sw2.ElapsedMilliseconds}");
            result.AppendText($"\r\nTime for One thread: {sw.ElapsedMilliseconds}");







            long many = sw2.ElapsedMilliseconds;
            long one = sw.ElapsedMilliseconds;
            Result.AppendText($"\r\nTime for Threads: {many}");
            Result.AppendText($"\r\nTime for One thread: {one - 700}");
        }

        
    }
}
