using System.ComponentModel;

namespace Forms
{
    public partial class Form1 : Form
    {
        private BackgroundWorker ballWorker = new BackgroundWorker();
        private BackgroundWorker sinusoidWorker = new BackgroundWorker();
        private BackgroundWorker rectangleWorker = new BackgroundWorker();
        private BackgroundWorker clockWorker = new BackgroundWorker();
        public Form1()
        {
            InitializeComponent();
            // ball
            ballWorker.DoWork += BallWorker_DoWork;
            ballWorker.ProgressChanged += ballWorker_ProgressChanged;
            ballWorker.WorkerReportsProgress = true;
            ballWorker.RunWorkerAsync();
            // sin
            sinusoidWorker.DoWork += SinusoidWorker_DoWork;
            sinusoidWorker.WorkerReportsProgress = true;
            sinusoidWorker.RunWorkerAsync();
            // rectangle
            rectangleWorker.DoWork += RectangleRepresentation_DoWork;
            rectangleWorker.WorkerReportsProgress = true;
            rectangleWorker.RunWorkerAsync();
            // clock
            clockWorker.DoWork += CustomRepresentation_DoWork;
            clockWorker.WorkerReportsProgress = true;
            clockWorker.RunWorkerAsync();
        }

        private bool isBallRunning = true;
        private void continue1_Click(object sender, EventArgs e)
        {
            isBallRunning = true;
        }

        private void stop1_Click(object sender, EventArgs e)
        {
            isBallRunning = false;
        }

        private int ballX = 50;
        private int ballY = 50;
        private int ballRadius = 20;

        private void BallWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (isBallRunning)
                {
                    if (ballY + ballRadius < ballPanel.Height)
                    {
                        ballY += 1;
                    }
                    else
                    {
                        ballY = 0;
                    }
                    ballWorker.ReportProgress(0);
                    Thread.Sleep(50);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }
        private void ballWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ballPanel.Invalidate();
        }
        private void ballPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            e.Graphics.FillEllipse(Brushes.Red, ballX, ballY, ballRadius, ballRadius);
        }

        // Global flag for sinusoid thread state
        private bool isSinusoidRunning = true;
        private int amplitude = 50;
        private int frequency = 5;
        private int xOffset = 0;

        private void continue3_Click(object sender, EventArgs e)
        {
            isSinusoidRunning = true;
        }

        private void stop3_Click(object sender, EventArgs e)
        {
            isSinusoidRunning = false;
        }

        private void SinusoidWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (isSinusoidRunning)
                {
                    xOffset += 1; 

                    Thread.Sleep(50); 
                    Invoke(new Action(() => sinusPanel.Invalidate()));
                }
                else
                {
                    Thread.Sleep(100);
                }
            }

        }
        private void sinusoidWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            sinusPanel.Invalidate();
        }
        private void sinusPanel_Paint(object sender, PaintEventArgs e)
        {
            DrawSinusoid(e.Graphics);
        }
        private void DrawSinusoid(Graphics g)
        {
            g.Clear(Color.White);
            double y = amplitude * Math.Sin(2 * Math.PI * frequency * (xOffset / (double)sinusPanel.Width));
            int currentYPos = sinusPanel.Height / 2 + (int)y;

            g.FillEllipse(Brushes.Green, Convert.ToInt32(xOffset), currentYPos, 10, 10);

            if (xOffset >= sinusPanel.Width)
            {
                g.Clear(SystemColors.Control);
                xOffset = 0;
            }
        }



        // Global flag for rectangle thread state
        private bool isRectangleRunning = true;
        private int kk = 0;
        private bool Forward = true;
        private void continue2_Click(object sender, EventArgs e)
        {
            isRectangleRunning = true;
        }

        private void stop2_Click(object sender, EventArgs e)
        {
            isRectangleRunning = false;
        }

        private void RectangleRepresentation_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (isRectangleRunning)
                {
                    if (kk >= rectanglePanel.Height)
                        Forward = false;
                    else if (kk == 0)
                        Forward = true;
                    if (Forward)
                        kk++;
                    else
                        kk--;
                    Thread.Sleep(50);
                    Invoke(new Action(() => rectanglePanel.Invalidate())); 
                }
                else
                {
                    Thread.Sleep(50); 
                }

            }
        }
        private void rectanglePanel_Paint(object sender, PaintEventArgs e)
        {
            DrawRectangle(e.Graphics);
        }
        private void DrawRectangle(Graphics g)
        {
            Pen pen = new Pen(Color.Black, 1);

            g.Clear(Color.White);
            g.DrawRectangle(pen, 0, 0, kk, kk);
        }


        // Global flag for circle thread state
        private bool isCustomRunning = true;
        private double angle = 0;
        private double minute = 0;

        private void continue4_Click(object sender, EventArgs e)
        {
            isCustomRunning = true;
        }

        private void stop4_Click(object sender, EventArgs e)
        {
            isCustomRunning = false;
        }

        private void CustomRepresentation_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {

                if (isCustomRunning)
                {
                    minute++;
                    angle = 2 * Math.PI * minute / 60;
                    if (minute == 60)
                    {
                        minute = 0;
                        angle = 0;
                    }
                    Thread.Sleep(1);
                    Invoke(new Action(() => circlePanel.Invalidate())); // Trigger the Paint event to redraw the sinusoidal wave
                }
                else
                {
                    Thread.Sleep(50); 
                }
            }
        }

        private void clockPanel_Paint(object sender, PaintEventArgs e)
        {
            DrawClock(e.Graphics);
        }
        private void DrawClock(Graphics g)
        {
            Pen pen = new Pen(Color.Blue, 1);
            int radius = Math.Min(circlePanel.Width, circlePanel.Height) / 2 - 10; 

            int centerX = circlePanel.Width / 2; 
            int centerY = circlePanel.Height / 2;

            g.Clear(Color.White);

            int x = (int)(centerX + radius * Math.Cos(angle));
            int y = (int)(centerY + radius * Math.Sin(angle));
            g.DrawLine(pen, centerX, centerY, x, y);
        }
    }
}