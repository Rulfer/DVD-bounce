using Python_Loader.Helpers;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace Python_Loader
{
    public partial class MainGUI : Form
    {
        private const int CornerRadius = 10;


        /// <summary>
        /// True while tue mouse is clicked down on this program.
        /// </summary>
        private bool _mouseDown;
        /// <summary>
        /// Position of this program when the user clicks on it. Used to calculate click-and-drag for window.
        /// </summary>
        private Point _lastLocation;

        public MainGUI()
        {
            InitializeComponent();

            btnLaunchProgram.HideInvoke();
            btnCloseProgram.HideInvoke();

            btnLaunchProgram.Click += OnLaunchProgramClicked;
            btnCloseProgram.Click += OnCloseProgramClicked;

            this.Paint += OnPaint;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            LabelProcess.Text = "Loading python";
            //RoundCorners(this);
            Program.OnFormLoaded();
        }

        /// <summary>
        /// We can dynamically set the backgroud to something with a gradient here.
        /// </summary>
        private void OnPaint(Object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            // Enable anti-aliasing
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

            Rectangle gradientRectangle = new Rectangle(0, 0, Width, Height);
            Color start = Color.FromArgb(255, 2, 242, 222);
            Color end = Color.FromArgb(255, 2, 200, 222);

            //Brush brush = new LinearGradientBrush(gradientRectangle, start, end, 45f);
            //graphics.FillRectangle(brush, gradientRectangle);

            using (Brush brush = new LinearGradientBrush(gradientRectangle, start, end, 45f))
            {
                // Draw the rounded corners to visually smooth out the edges
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(new Rectangle(0, 0, CornerRadius, CornerRadius), 180, 90);
                    path.AddArc(new Rectangle(this.Width - CornerRadius, 0, CornerRadius, CornerRadius), 270, 90);
                    path.AddArc(new Rectangle(this.Width - CornerRadius, this.Height - CornerRadius, CornerRadius, CornerRadius), 0, 90);
                    path.AddArc(new Rectangle(0, this.Height - CornerRadius, CornerRadius, CornerRadius), 90, 90);
                    path.CloseAllFigures();

                    // Apply the rounded corners to the form
                    graphics.FillPath(brush, path);

                    this.Region = new Region(path);
                }

            }


            //Brush brush = new LinearGradientBrush(gradientRectangle, start, end, 45f);
            //graphics.FillRectangle(brush, gradientRectangle);
        }

        private void RoundCorners(Form form)
        {
            // Create a new GraphicsPath
            GraphicsPath path = new GraphicsPath();

            // Define the rectangle for the rounded corners
            path.AddArc(new Rectangle(0, 0, CornerRadius, CornerRadius), 180, 90);
            path.AddArc(new Rectangle(form.Width - CornerRadius, 0, CornerRadius, CornerRadius), 270, 90);
            path.AddArc(new Rectangle(form.Width - CornerRadius, form.Height - CornerRadius, CornerRadius, CornerRadius), 0, 90);
            path.AddArc(new Rectangle(0, form.Height - CornerRadius, CornerRadius, CornerRadius), 90, 90);
            path.CloseAllFigures();

            // Apply the rounded corners to the form
            form.Region = new Region(path);
        }

        public void OnPythonLoaded()
        {
            LabelProcess.TextInvoke("PIP Processing");
        }

        public void OnPipReady()
        {
            LabelProcess.TextInvoke("App ready");
            btnLaunchProgram.ShowInvoke();
        }

        public void OnPythonClosed()
        {
            btnLaunchProgram.ShowInvoke();
            btnCloseProgram.HideInvoke();
        }

        internal void SetLabelText(string text)
        {
            LabelProcess.TextInvoke(text);
        }

        #region UI events
        private void OnLaunchProgramClicked(object? sender, EventArgs e)
        {
            Program.LoadPythonProgram();
            btnLaunchProgram.Hide();
            btnCloseProgram.Show();
        }

        private void OnCloseProgramClicked(object? sender, EventArgs e)
        {
            Program.CloseProcess();
            OnPythonClosed();
        }
        #endregion

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _mouseDown = true;
            _lastLocation = e.Location;
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _mouseDown = false;
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - _lastLocation.X) + e.X, (this.Location.Y - _lastLocation.Y) + e.Y);

                this.Update();
            }
            base.OnMouseMove(e);
        }
    }
}
