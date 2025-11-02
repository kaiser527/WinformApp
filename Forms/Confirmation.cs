using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormApp.Forms
{
    public partial class Confirmation : Form
    {
        public bool Result { get; private set; } = false;

        public Confirmation(string title, string message)
        {
            InitializeComponent();
            HideFormControlBar();
            StyleButtons();
            SetTitleAndMessage(title, message);
        }
        #region Method
        private void HideFormControlBar()
        {
            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;
            Text = string.Empty;
        }
        private void SetTitleAndMessage(string title, string message)
        {
            lblTitle.Text = title;
            lblMessage.Text = message;
        }
        private void StyleButtons()
        {
            UIStyles.ModernUIButton(btnOK, Color.FromArgb(46, 204, 113), Color.FromArgb(22, 160, 133));
            UIStyles.ModernUIButton(btnCancel, Color.FromArgb(231, 76, 60), Color.FromArgb(176, 52, 40));
        }
        public static bool ShowConfirm(string title, string message)
        {
            using (var form = new Confirmation(title, message))
            {
                form.ShowDialog();
                return form.Result;
            }
        }
        #endregion
        #region Event
        private void btnOK_Click(object sender, EventArgs e)
        {
            Result = true;
            Close();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Result = false;
            Close();
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Opacity = 0;
            Timer t = new Timer();
            t.Interval = 10;
            t.Tick += (s, ev) =>
            {
                if (Opacity >= 1) t.Stop();
                Opacity += 0.05;
            };
            t.Start();
        }
        #endregion
    }
}
