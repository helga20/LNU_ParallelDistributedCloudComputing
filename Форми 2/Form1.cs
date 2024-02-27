namespace Parallel4
{
    public partial class Form1 : Form
    {
        public Form1()
        {

            InitializeComponent();

            Button ball = new Button();
            ball.Text = "Ball";
            ball.Size = new Size(100, 50);
            ball.Location = new Point(10, 0);
            ball.Click += OpenFormBall_Click;
            this.Controls.Add(ball);

            Button triangle = new Button();
            triangle.Text = "Triangle";
            triangle.Size = new Size(100, 50);
            triangle.Location = new Point(10, 60);
            triangle.Click += OpenFormTriangle_Click;
            this.Controls.Add(triangle);

            Button sinus = new Button();
            sinus.Text = "Sinus";
            sinus.Size = new Size(100, 50);
            sinus.Location = new Point(10, 120);
            sinus.Click += OpenFormSinus_Click;
            this.Controls.Add(sinus);

            Button square = new Button();
            square.Text = "Square";
            square.Size = new Size(100, 50);
            square.Location = new Point(10, 180);
            square.Click += OpenFormSquare_Click;
            this.Controls.Add(square);

        }


        private void Form1_Load(object sender, EventArgs e)
        {


        }
        private void OpenFormBall_Click(object sender, EventArgs e)
        {
            FormBall formBall = new FormBall();
            formBall.Show();
        }
        private void OpenFormTriangle_Click(object sender, EventArgs e)
        {
            TriangleForm formtr = new TriangleForm();
            formtr.Show();
        }
        private void OpenFormSinus_Click(object sender, EventArgs e)
        {
            FormSine formSine = new FormSine();
            formSine.Show();
        }
        private void OpenFormSquare_Click(object sender, EventArgs e)
        {
            FormSquare formSquare = new FormSquare();
            formSquare.Show();
        }

    }
}