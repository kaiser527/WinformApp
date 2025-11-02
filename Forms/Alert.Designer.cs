using System.Drawing;
using System.Windows.Forms;

namespace WinFormApp.Forms
{
    partial class Alert
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblMessage;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblMessage = new Label();
            this.SuspendLayout();

            // lblMessage
            this.lblMessage.Dock = DockStyle.Fill;
            this.lblMessage.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblMessage.ForeColor = Color.White;
            this.lblMessage.TextAlign = ContentAlignment.MiddleCenter;

            // Alert
            this.ClientSize = new Size(300, 60);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.BackColor = Color.DarkGray;
            this.Opacity = 0.95D;
            this.TopMost = true;

            this.ResumeLayout(false);
        }
    }
}
