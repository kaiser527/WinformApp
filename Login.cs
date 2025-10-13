using System;
using System.Windows.Forms;
using WinFormApp.Services;

namespace WinFormApp
{
    public partial class Login : Form
    {
        private bool isLoading;
        private bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;
                btnLogin.Enabled = !value;      
                btnLogin.Text = value ? "Loading..." : "Login"; 
                Cursor = value ? Cursors.WaitCursor : Cursors.Default; 
            }
        }

        public Login()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (IsLoading) return; // Prevent clicking twice quickly

            string username = textBoxUsername.Text.Trim();
            string password = textBoxPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.", "Missing information");
                return;
            }

            IsLoading = true;

            try
            {
                var account = await AccountService.Instance.Login(username, password);

                if (account != null && account.IsAuthenticated)
                {
                    TableManager tableManager = new TableManager();
                    Hide();
                    tableManager.ShowDialog();
                    Show();
                }
                else
                {
                    MessageBox.Show("Incorrect username or password", "Login failed");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Login error");
            }
            finally
            {
                IsLoading = false;
                btnLogin.Text = "Login";
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit the program?",
                "Exit Confirmation",
                MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
               e.Cancel = true;
            }
        }
    }
}
