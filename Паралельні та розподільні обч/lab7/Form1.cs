using System.Diagnostics;
using System.Text;

namespace lab7
{
    public partial class Form1 : Form
    {
        private Label[] labels = new Label[4];

        public string StartFile = @"C:\Users\Oksana\RiderProjects\Паралельні та розподільні обч\lab7\StartData.txt";
        public string EndFile = @"C:\Users\Oksana\RiderProjects\Паралельні та розподільні обч\lab7\EndData.txt";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
       

        private void ThreadsActivity_TextChanged(object sender, EventArgs e)
        {

        }

        private void MainThread_Click(object sender, EventArgs e)
        {
            ThreadsActivity.AppendText("\r\nMain thread started");
            
            string[] lines = File.ReadAllLines(StartFile);
            MyClass readClass = new MyClass();
            foreach(string line in lines)
            {
                File.AppendAllText(EndFile, line);
                string res = "[" + readClass.StringCount(line) + "]\n";
                File.AppendAllText(EndFile, res);
            }
            ThreadsActivity.AppendText("\r\nMain thread ended");
        }
        async Task WriteTextAsync(string filePath, string text)
        {
            //byte[] encodedText = Encoding.Unicode.GetBytes(text);
            byte[] encodedText = Encoding.ASCII.GetBytes(text);
            using var sourceStream =
                new FileStream(
                    filePath,
                    FileMode.Create, FileAccess.Write, FileShare.None,
                    bufferSize: 4096, useAsync: true);

            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        }

        private void ManyThreads_Click(object sender, EventArgs e)
        {
            
            string[] lines = File.ReadAllLines(StartFile);
            MyClass readClass = new MyClass();
            List<Task> threads = new List<Task>();
            string endResult = "";
            for (int i = 0; i < lines.Length; i++)
            {
                Task task = new Task(o =>
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    int c = (int)o;
                    ThreadsActivity.AppendText($"\r\n{c} thread started");
                   
                    endResult += $"{lines[c]}[" + readClass.StringCount(lines[c]) + "]\n";

                    sw.Stop();
                    ThreadsActivity.AppendText($"\r\n{c} thread ended with time: {sw.ElapsedMilliseconds}");
                    

                    

                }, i);
                threads.Add(task);
                task.Start();
            }
            Task.WaitAll(threads.ToArray());
            File.AppendAllText(EndFile,endResult);


            

        }
    }
    public class MyClass
    {
        public string StringCount(string s)
        {
            uint result = 0;
            foreach (char c in s)
            {
                result += (UInt32)c;
            }
            return result.ToString();
        }


        public string WriteInFile(string line, TextBox t,  int i)
        {
            return $"{line} [" + StringCount(line) + "]\n";
            t.AppendText($"\r\n{i} thread ended");
        }



    }
}