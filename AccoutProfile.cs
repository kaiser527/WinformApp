using System.Windows.Forms;
using WinFormApp.DTO;
using WinFormApp.Models;
using WinFormApp.Services;

namespace WinFormApp
{
    public partial class AccoutProfile : Form
    {
        private readonly TableManager _tableManager;

        public AccoutProfile(TableManager tableManager)
        {
            InitializeComponent();
            _tableManager = tableManager;
            ChangeAccount();
        }

        private void ChangeAccount()
        {
            Account user = AccountService.Instance.User;

            textBoxUsername.Text = user.UserName;
            textBoxDisplayName.Text = user.DisplayName;
        }

        private async void btnUpdate_Click(object sender, System.EventArgs e)
        {
            UpdateAccountDTO updateAccountDTO = 
                new UpdateAccountDTO(
                    textBoxUsername.Text, 
                    textBoxDisplayName.Text, 
                    textBoxPassword.Text,
                    textBoxNewPassword.Text,
                    textBoxReEnter.Text
                );

            (bool isClose, Account updatedUser) = await AccountService.Instance.UpdateAccount(updateAccountDTO);

            if(isClose && updatedUser != null)
            {
                _tableManager.accountToolStripDropdown.Text = $"Account ({updatedUser.DisplayName})";

                Close();
            }
        }

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
