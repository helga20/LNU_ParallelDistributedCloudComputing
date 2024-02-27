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
    public partial class FormSine : Form
    {
        private Thread _sineWaveThread;
        private ManualResetEvent _sineWaveThreadPause = new ManualResetEvent(true);
        private bool _sineWaveThreadStop;

        private float _sineWavePhase = 0;
        private float _sineWaveAmplitude = 50;
        public FormSine()
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

            _sineWaveThread = new Thread(AnimateSineWave) { IsBackground = true };
            _sineWaveThread.Start();
        }

        private void FormSine_Load(object sender, EventArgs e)
        {

        }

        private void AnimateSineWave()
        {
            while (!_sineWaveThreadStop)
            {
                _sineWaveThreadPause.WaitOne();
                _sineWavePhase += 5; 

                if (_sineWavePhase >= 360)
                {
                    _sineWavePhase -= 360;
                }

                Invoke(new Action(Invalidate));
                Thread.Sleep(50);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            List<PointF> points = new List<PointF>();

            for (int x = 0; x < this.ClientSize.Width; x++)
            {
                float sineValue = (float)Math.Sin((x + _sineWavePhase) * 2 * Math.PI / 180) * _sineWaveAmplitude;
                float y = this.ClientSize.Height / 2 + sineValue;
                points.Add(new PointF(x, y));
            }

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (Pen pen = new Pen(Color.Green, 2))
            {
                e.Graphics.DrawLines(pen, points.ToArray());
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            _sineWaveThreadPause.Reset();
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            _sineWaveThreadPause.Set();
        }

        private void FormSine_FormClosing(object sender, FormClosingEventArgs e)
        {
            _sineWaveThreadStop = true;
        }
    }
}
