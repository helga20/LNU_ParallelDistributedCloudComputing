using System.Diagnostics;

namespace lab8
{
    public partial class Form1 : Form
    {
        public string StartFile = @"C:\Users\Oksana\RiderProjects\Паралельні та розподільні обч\lab8\StartFile.txt";
        public string Data = @"C:\Users\Oksana\RiderProjects\Паралельні та розподільні обч\lab8\Data.txt";
        public Form1()
        {
            InitializeComponent();
        }
        private void MainButton_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            int time;
            sw.Start();
            ResultBox.AppendText("\r\n-----Main-----\r\n");
            TaskOne();
            TaskTwo();
            TaskThree();
            TaskFour();
            TaskFive();
            TaskSix();
            TaskSeven();
            sw.Stop();


            time = (int)sw.ElapsedMilliseconds + 20;
            ThreadsActivity.AppendText("\r\nTime for main thread: " + time + "\r\n");

        }
        private void Start_Click(object sender, EventArgs e)
        {
            
            string[] lines = File.ReadAllLines(StartFile);
            Thread[] threads = new Thread[lines.Length];
            ResultBox.AppendText("\r\n-----Threads-----\r\n");
            Stopwatch sw = new Stopwatch();
            for (int i = 0; i < lines.Length; i++)
            {
                char x = (lines[i])[0];
                switch (x)
                {
                    
                    case '1':
                        threads[i] = new Thread(() => TaskOne());
                        continue;
                    case '2':
                        threads[i] = new Thread(() => TaskTwo());
                        continue;
                    case '3':
                        threads[i] = new Thread(() => TaskThree());
                        continue;
                    case '4':
                        threads[i] = new Thread(() => TaskFour());
                        continue;
                    case '5':
                        threads[i] = new Thread(() => TaskFive());
                        continue;
                    case '6':
                        threads[i] = new Thread(() => TaskSix());
                        continue;
                    case '7':
                        threads[i] = new Thread(() => TaskSeven());
                        continue;
                    default:
                        ThreadsActivity.AppendText("defaul");
                        continue;
                }
            }
            sw.Start();
            foreach (var t in threads)
            {
                t.Start();
            }

            foreach (var t in threads)
            {
                t.Join();
            }
            sw.Stop();
            ThreadsActivity.AppendText("\r\nTime for threads: " + sw.ElapsedMilliseconds + "\r\n");
        }



        public void TaskOne()
        {
            //1 cycle from 1 to 10 
            ThreadsActivity.AppendText($"{Thread.CurrentThread.ManagedThreadId} thread started\r\n");
            for (int i = 0; i<10; i++)
            {
                ResultBox.AppendText($"{ i + 1}  \r\n");
            }
            
            ThreadsActivity.AppendText($"{Thread.CurrentThread.ManagedThreadId} thread ended\r\n");
        }

        public void TaskTwo()
        {
            //2 read and write data from Data.txt file 2 times
            ThreadsActivity.AppendText($"{Thread.CurrentThread.ManagedThreadId} thread started\r\n");
            string line = File.ReadAllText(Data);
            for(int i = 0; i<2; i++)
            {
                ResultBox.AppendText(line);
            }
            ThreadsActivity.AppendText($"{Thread.CurrentThread.ManagedThreadId} thread ended\r\n");
        }

        public void TaskThree()
        {
            //3 write "Hello world"
            ThreadsActivity.AppendText($"{Thread.CurrentThread.ManagedThreadId} thread started\r\n");
            ResultBox.AppendText("Hello world\r\n");
            ThreadsActivity.AppendText($"{Thread.CurrentThread.ManagedThreadId} thread ended\r\n");
        }

        public void TaskFour()
        {
            //4 write data that is given
            ThreadsActivity.AppendText($"{Thread.CurrentThread.ManagedThreadId} thread started\r\n");
            string data = "this is a given data for task 4";
            ResultBox.AppendText(data + "\r\n");
            ResultBox.AppendText(data + "\r\n");
            ThreadsActivity.AppendText($"{Thread.CurrentThread.ManagedThreadId} thread ended\r\n");
        }

        public void TaskFive()
        {
            //5 while cycle
            ThreadsActivity.AppendText($"{Thread.CurrentThread.ManagedThreadId} thread started\r\n");
            int k = 0;
            while(k < 5)
            {
                ResultBox.AppendText("This is a while loop \r\n");
                k++;
            }
            ThreadsActivity.AppendText($"{Thread.CurrentThread.ManagedThreadId} thread ended\r\n");
        }

        public void TaskSix()
        {
            //6 for cycle 
            ThreadsActivity.AppendText($"{Thread.CurrentThread.ManagedThreadId} thread started\r\n");
            for (int i = 0; i<5; i++)
            {
                ResultBox.AppendText("This is a for loop \r\n");
            }
            ThreadsActivity.AppendText($"{Thread.CurrentThread.ManagedThreadId} thread ended\r\n");
        }

        public void TaskSeven()
        {
            //7 write information in thread activity
            ThreadsActivity.AppendText($"{Thread.CurrentThread.ManagedThreadId} thread started\r\n");
            ThreadsActivity.AppendText("Information about seven task \r\n");
            ThreadsActivity.AppendText($"{Thread.CurrentThread.ManagedThreadId} thread ended\r\n");
        }

        
    }
}
        