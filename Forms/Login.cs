using System;
using System.Windows.Forms;
using WinFormApp.Forms;
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
                Alert.ShowAlert("Missing information", Alert.AlertType.Warning);
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
                    Alert.ShowAlert("Incorrect username or password", Alert.AlertType.Error);
                }
            }
            catch (Exception ex)
            {
                Alert.ShowAlert($"Error: {ex.Message}", Alert.AlertType.Error);
            }
            finally
            {
                IsLoading = false;
                btnLogin.Text = "Login";
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool confirm = Confirmation.ShowConfirm("Exit Confirmation",
               "Are you sure you want to exit the program?");

            if (!confirm) e.Cancel = true;
        }
    }
}
