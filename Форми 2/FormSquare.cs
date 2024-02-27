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
    public partial class FormSquare : Form
    {
        private Thread _squareThread;
        private ManualResetEvent _squareThreadPause = new ManualResetEvent(true);
        private bool _squareThreadStop;

        private int _squareSize = 50;
        private int _squareScalingDirection = 1;
        public FormSquare()
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

            _squareThread = new Thread(AnimateSquare) { IsBackground = true };
            _squareThread.Start();
        }

        private void FormSquare_Load(object sender, EventArgs e)
        {

        }
        private void AnimateSquare()
        {
            while (!_squareThreadStop)
            {
                _squareThreadPause.WaitOne();

                _squareSize += 5 * _squareScalingDirection;

                if (_squareSize >= 200 || _squareSize <= 50)
                {
                    _squareScalingDirection *= -1;
                }

                Invoke(new Action(Invalidate));
                Thread.Sleep(50);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int squarePosX = (this.ClientSize.Width - _squareSize) / 2;
            int squarePosY = (this.ClientSize.Height - _squareSize) / 2;
            Rectangle squareRect = new Rectangle(squarePosX, squarePosY, _squareSize, _squareSize);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (SolidBrush brush = new SolidBrush(Color.Purple))
            {
                e.Graphics.FillRectangle(brush, squareRect);
            }
        }
        private void btnPause_Click(object sender, EventArgs e)
        {
            _squareThreadPause.Reset();
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            _squareThreadPause.Set();
        }

        private void FormCustom_FormClosing(object sender, FormClosingEventArgs e)
        {
            _squareThreadStop = true;
        }
    }
}
