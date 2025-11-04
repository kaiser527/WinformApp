using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormApp.Forms
{
    public partial class Alert : Form
    {
        public enum AlertType { Success, Warning, Error, Info }

        private int x, y;
        private Timer timerClose;
        private Timer timerShow;

        public Alert(string message, AlertType type)
        {
            InitializeComponent();
            SetAlertStyle(type);
            AnimationEffect();

            TopMost = true;
            lblMessage.Text = message;
        }

        #region Methods
        private void AnimationEffect()
        {
            // Position top right
            x = Screen.PrimaryScreen.WorkingArea.Width - Width - 10;
            y = Screen.PrimaryScreen.WorkingArea.Height - Height - 10;

            // start slightly lower for slide-up animation
            Location = new Point(x, y + 100);

            // Fade & slide
            timerShow = new Timer
            {
                Interval = 1
            };
            timerShow.Tick += TimerShow_Tick;
            timerShow.Start();

            // Auto close
            timerClose = new Timer
            {
                Interval = 3000
            };
            timerClose.Tick += TimerClose_Tick;
            timerClose.Start();
        }

        private void SetAlertStyle(AlertType type)
        {
            switch (type)
            {
                case AlertType.Success:
                    BackColor = Color.FromArgb(46, 204, 113);
                    break;
                case AlertType.Warning:
                    BackColor = Color.FromArgb(241, 196, 15);
                    break;
                case AlertType.Error:
                    BackColor = Color.FromArgb(231, 76, 60);
                    break;
                case AlertType.Info:
                    BackColor = Color.FromArgb(52, 152, 219);
                    break;
            }
        }

        public static void ShowAlert(string msg, AlertType type = AlertType.Info)
        {
            Alert frm = new Alert(msg, type);
            frm.Show();
        }
        #endregion

        #region Events
        private void TimerShow_Tick(object sender, EventArgs e)
        {
            if (Top > y)
            {
                Top -= 8; 
            }
            else
            {
                timerShow.Stop();
            }
        }

        private void TimerClose_Tick(object sender, EventArgs e)
        {
            timerClose.Stop();
            Close();
        }
        #endregion
    }
}
