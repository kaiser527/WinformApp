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
        }

        #region Method
        private void ChangeAccount()
        {
            Account user = AccountService.Instance.User;

            textBoxUsername.Text = user.UserName;
            textBoxDisplayName.Text = user.DisplayName;
        }

        private void LoadAccountImageToButtonUpload()
        {
            Account user = AccountService.Instance.User;

            ImageService.Instance.LoadAccountImageToButton(user, btnUploadAccount);
        }
        #endregion

        #region Event
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

            if(isClose && updatedUser != null)
            {
                _tableManager.accountToolStripDropdown.Text = $"Account ({updatedUser.DisplayName})";

                Image img = ImageService.Instance.LoadAccountImage(updatedUser, 38);
                if (img == null)
                {
                    MessageBox.Show("Image not found or failed to load!");
                }
                else
                {
                    _tableManager.AccountImage.Image = img;
                    _tableManager.AccountImage.SizeMode = PictureBoxSizeMode.Zoom;
                }

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
