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
    public partial class TriangleForm : Form
    {
        private Thread _triangleThread;
        private ManualResetEvent _triangleThreadPause = new ManualResetEvent(true);
        private bool _triangleThreadStop;

        private float _triangleRotation = 0;
        public TriangleForm()
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

            _triangleThread = new Thread(SpinTriangle) { IsBackground = true };
            _triangleThread.Start();
        }

        private void TriangleForm_Load(object sender, EventArgs e)
        {

        }
        private void SpinTriangle()
        {
            while (!_triangleThreadStop)
            {
                _triangleThreadPause.WaitOne();
                _triangleRotation += 5; 

                Invoke(new Action(Invalidate));
                Thread.Sleep(50);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Define the triangle's vertices
            PointF[] points = new PointF[3]
            {
        new PointF(this.ClientSize.Width / 2, this.ClientSize.Height / 2 - 50),
        new PointF(this.ClientSize.Width / 2 - 50, this.ClientSize.Height / 2 + 50),
        new PointF(this.ClientSize.Width / 2 + 50, this.ClientSize.Height / 2 + 50)
            };

            // Calculate the transformation matrix for rotation
            System.Drawing.Drawing2D.Matrix rotationMatrix = new System.Drawing.Drawing2D.Matrix();
            rotationMatrix.RotateAt(_triangleRotation, new PointF(this.ClientSize.Width / 2, this.ClientSize.Height / 2));

            // Apply the transformation matrix to the triangle
            rotationMatrix.TransformPoints(points);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (SolidBrush brush = new SolidBrush(Color.Blue))
            {
                e.Graphics.FillPolygon(brush, points);
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            _triangleThreadPause.Reset();
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            _triangleThreadPause.Set();
        }

        private void FormRectangle_FormClosing(object sender, FormClosingEventArgs e)
        {
            _triangleThreadStop = true;
        }
    }
}
