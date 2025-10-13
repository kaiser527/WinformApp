using System;
using System.Drawing;
using System.IO;
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

            Image profileImage = AccountService.Instance.LoadAccountImage(user, 84);

            if (profileImage != null)
            {
                btnUploadAccount.BackgroundImage = profileImage;
                btnUploadAccount.BackgroundImageLayout = ImageLayout.Zoom;
                btnUploadAccount.Text = "";
            }
            else
            {
                btnUploadAccount.Text = "+";
                btnUploadAccount.BackgroundImage = null;
            }
        }

        private string SelectAndSaveAccountImage()
        {
            UploadAccountImage.Title = "Select Profile Image";
            UploadAccountImage.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            UploadAccountImage.Multiselect = false;

            if (UploadAccountImage.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = UploadAccountImage.FileName;
                string fileName = Path.GetFileName(selectedPath);

                string imageFolder = Path.Combine(Application.StartupPath, "Image");
                Directory.CreateDirectory(imageFolder);

                string destinationPath = Path.Combine(imageFolder, fileName);

                try
                {
                    if (File.Exists(destinationPath))
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                        File.SetAttributes(destinationPath, FileAttributes.Normal);
                        File.Delete(destinationPath);
                    }

                    File.Copy(selectedPath, destinationPath, overwrite: true);

                    MessageBox.Show($"Image '{fileName}' uploaded successfully!", "Upload Success");
                    return fileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Image upload failed: {ex.Message}", "Error");
                    return null;
                }
            }

            return null;
        }

        #endregion

        #region Event
        private async void btnUpdate_Click(object sender, System.EventArgs e)
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

                Image img = AccountService.Instance.LoadAccountImage(updatedUser, 38);
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

                Close();
            }
        }

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void btnUploadAccount_Click(object sender, EventArgs e)
        {
            string fileName = SelectAndSaveAccountImage();

            if (!string.IsNullOrEmpty(fileName))
            {
                _fileName = fileName;
                AccountService.Instance.User.Image = _fileName;

                string imageFolder = Path.Combine(Application.StartupPath, "Image");
                string imagePath = Path.Combine(imageFolder, _fileName);

                if (File.Exists(imagePath))
                {
                    using (var original = Image.FromFile(imagePath))
                    {
                        btnUploadAccount.BackgroundImage = new Bitmap(original, new Size(84, 84));
                    }

                    btnUploadAccount.BackgroundImageLayout = ImageLayout.Zoom;
                    btnUploadAccount.Text = "";

                    MessageBox.Show($"Image '{fileName}' uploaded successfully!", "Success");
                }
                else
                {
                    MessageBox.Show($"Image file not found: {imagePath}", "Error");
                }
            }
        }
        #endregion
    }
}
