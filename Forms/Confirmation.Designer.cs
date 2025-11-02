using System.Windows.Forms;

namespace WinFormApp.Forms
{
    partial class Confirmation
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private Label lblMessage;
        private Button btnOK;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // Title
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 14F);
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.Dock = DockStyle.Top;
            this.lblTitle.Height = 50;

            // Message
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblMessage.Dock = DockStyle.Top;
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMessage.Padding = new Padding(10);
            this.lblMessage.Height = 100;

            // OK Button
            this.btnOK.Text = "Confirm";
            this.btnOK.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnOK.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.btnOK.ForeColor = System.Drawing.Color.White;
            this.btnOK.FlatStyle = FlatStyle.Flat;
            this.btnOK.Size = new System.Drawing.Size(120, 35);
            this.btnOK.Location = new System.Drawing.Point(60, 154);
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);

            // Cancel Button
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.Size = new System.Drawing.Size(120, 35);
            this.btnCancel.Location = new System.Drawing.Point(220, 154);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // Form
            this.ClientSize = new System.Drawing.Size(400, 210);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblTitle);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.BackColor = System.Drawing.Color.White;
            this.Name = "Confirmation";

            this.ResumeLayout(false);
        }
    }
}
