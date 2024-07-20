using Python_Loader.Helpers;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

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

        Color _start = Color.FromArgb(255, 2, 242, 222);
        Color _end = Color.FromArgb(255, 2, 200, 222);

        public MainGUI()
        {
            InitializeComponent();

            // Enable double buffering to reduce flicker
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);

            btnLaunchProgram.HideInvoke();
            btnCloseProgram.HideInvoke();

            btnLaunchProgram.Click += OnLaunchProgramClicked;
            btnCloseProgram.Click += OnCloseProgramClicked;

            this.Paint += OnPaintBackground;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            LabelProcess.Text = "Loading python";
            Program.OnFormLoaded();
        }

        /// <summary>
        /// We can dynamically set the backgroud to something with a gradient here.
        /// </summary>
        private void OnPaintBackground(Object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            // Enable anti-aliasing
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

            Rectangle gradientRectangle = new Rectangle(0, 0, Width, Height);


            using (Brush brush = new LinearGradientBrush(gradientRectangle, _start, _end, 270))
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
        }

        //private void OnPaintButton(Object sender, PaintEventArgs e)
        //{
        //    Graphics graphics = e.Graphics;
        //    Button btn = sender as Button;
        //    Color currentColor = btn.BackColor;

        //    // Create a semi-transparent brush
        //    using (SolidBrush brush = new SolidBrush(Color.FromArgb(128, currentColor.R, currentColor.G, currentColor.B)))
        //    {
        //        e.Graphics.FillRectangle(brush, btn.ClientRectangle);
        //    }

        //    // Draw text
        //    ButtonRenderer.DrawButton(e.Graphics, btn.ClientRectangle, btn.Text, btn.Font, false, PushButtonState.Normal);
        //}

        //private void OnPaintButton(Object sender, PaintEventArgs e)
        //{
        //    Graphics graphics = e.Graphics;
        //    Button btn = sender as Button;

        //    if (btn != null)
        //    {
        //        using (LinearGradientBrush brush = new LinearGradientBrush(btn.ClientRectangle, _end, _start, 45F))
        //        {
        //            e.Graphics.FillRectangle(brush, btn.ClientRectangle);
        //        }
        //        ControlPaint.DrawBorder(e.Graphics, btn.ClientRectangle, Color.Gray, ButtonBorderStyle.Solid);
        //        //ButtonRenderer.DrawButton(e.Graphics, btn.ClientRectangle, btn.Text, btn.Font, false, PushButtonState.Normal);
        //        TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, btn.ClientRectangle, btn.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        //    }

        //}

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

        #region Drag Program events.
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
        #endregion

        private void MainGUI_DragEnter(object sender, DragEventArgs e)
        {
            Debug.WriteLine(this + " HELLO");
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string filePath in files)
                {
                    Debug.WriteLine(filePath);
                }
            }
        }
    }
}
