using System;
using System.Windows.Forms;
using WinFormApp.DTO;
using WinFormApp.Services;

namespace WinFormApp
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = textBoxUsername.Text;
            string password = textBoxPassword.Text;

            AccountDTO account = await AccountService.Instance.Login(username, password);

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
