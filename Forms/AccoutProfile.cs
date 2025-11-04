using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormApp.DTO;
using WinFormApp.Models;
using WinFormApp.Services;

namespace WinFormApp
{
    public partial class AccoutProfile : Form
    {
        private readonly TableManager _tableManager;

        private string _fileName;

        public AccoutProfile(TableManager tableManager)
        {
            InitializeComponent();
            _tableManager = tableManager;
            ChangeAccount();
            LoadAccountImageToButtonUpload();
            StyleButtons();
            StylePanels();
        }

        #region Methods
        private void StyleButtons()
        {
            UIStyles.ModernUIButton(btnUpdate, Color.FromArgb(52, 152, 219), Color.FromArgb(35, 110, 160));
            UIStyles.ModernUIButton(btnExit, Color.FromArgb(231, 76, 60), Color.FromArgb(176, 52, 40));
        }

        private void ChangeAccount()
        {
            Account user = AccountService.Instance.User;

            textBoxUsername.Text = user.UserName;
            textBoxDisplayName.Text = user.DisplayName;
        }

        private void StylePanels()
        {
            int radius = 15; 

            foreach (Panel pnl in new[] { panel1, panel2, panel4, panel5, panel6, panel7, panel8, panel9 })
            {
                UIStyles.RoundPanel(pnl, radius);              
            }
        }

        private void LoadAccountImageToButtonUpload()
        {
            Account user = AccountService.Instance.User;

            ImageService.Instance.LoadAccountImageToButton(user, btnUploadAccount);

            _fileName = user.Image;
        }
        #endregion

        #region Events
        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateAccountDTO updateAccountDTO =
                new UpdateAccountDTO(
                    textBoxUsername.Text,
                    textBoxDisplayName.Text,
                    textBoxPassword.Text,   
                    textBoxNewPassword.Text,
                    textBoxReEnter.Text,
                    _fileName
                );

            (bool isClose, Account updatedUser) = await AccountService.Instance.UpdateAccount(updateAccountDTO);

            if (isClose && updatedUser != null)
            {
                _tableManager.accountToolStripDropdown.Text = $"Account ({updatedUser.DisplayName})";

                AccountService.Instance.User = updatedUser;
                AccountService.Instance.User.Image = _fileName;

                Close();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnUploadAccount_Click(object sender, EventArgs e)
        {
            string fileName = ImageService.Instance.UploadAccountImageDialog(UploadAccountImage);

            if (!string.IsNullOrEmpty(fileName))
            {
                _fileName = fileName;
                ImageService.Instance.UpdateButtonImage(fileName, btnUploadAccount);
            }
        }
        #endregion
    }
}
