using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parallel4
{
    public partial class FormBall : Form
    {
        private Thread _ballThread;
        private ManualResetEvent _ballThreadPause = new ManualResetEvent(true);
        private bool _ballThreadStop;

        private const int BallRadius = 40;
        private int _ballPosX = 0;
        private int _ballSpeed = 5;
        public FormBall()
        {
            InitializeComponent();

            Button btnPause = new Button();
            btnPause.Text = "Pause";
            btnPause.Location = new Point(10, 10);
            btnPause.Click += btnPause_Click;
            this.Controls.Add(btnPause);

            Button btnResume = new Button();
            btnResume.Text = "Resume";
            btnResume.Location = new Point(10, 50);
            btnResume.Click += btnResume_Click;
            this.Controls.Add(btnResume);

            _ballThread = new Thread(BallAnimation) { IsBackground = true };
            _ballThread.Start();
        }

        private void BallAnimation()
        {
            while (!_ballThreadStop)
            {
                _ballThreadPause.WaitOne();

                _ballPosX += _ballSpeed;

                if (_ballPosX >= this.Width - BallRadius * 2)
                {
                    _ballPosX = this.Width - BallRadius * 2;
                    _ballSpeed = -_ballSpeed;
                }
                else if (_ballPosX <= 0)
                {
                    _ballPosX = 0;
                    _ballSpeed = -_ballSpeed;
                }

                Invoke(new Action(Invalidate));
                Thread.Sleep(50);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            float posY = (this.ClientSize.Height / 2) - BallRadius;
            RectangleF ballRect = new RectangleF(_ballPosX, posY, BallRadius * 2, BallRadius * 2);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (SolidBrush brush = new SolidBrush(Color.Red))
            {
                e.Graphics.FillEllipse(brush, ballRect);
            }
        }

        private void FormBall_Load(object sender, EventArgs e)
        {

        }
        private void btnPause_Click(object sender, EventArgs e)
        {
            _ballThreadPause.Reset();
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            _ballThreadPause.Set();
        }

        private void FormBall_FormClosing(object sender, FormClosingEventArgs e)
        {
            _ballThreadStop = true;
        }
    }
}
