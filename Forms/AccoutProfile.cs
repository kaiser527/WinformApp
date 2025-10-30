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

        #region Method
        private void StyleButtons()
        {
            foreach (Button btn in new[] { btnUpdate, btnExit, btnUploadAccount })
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.ForeColor = Color.White;
                btn.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                btn.Cursor = Cursors.Hand;

                btn.Region = Region.FromHrgn(
                    CreateRoundRectRgn(0, 0, btn.Width, btn.Height, 15, 15)
                );
            }

            btnUpdate.BackColor = Color.FromArgb(52, 152, 219);
            btnUpdate.MouseEnter += (s, e) => btnUpdate.BackColor = Color.FromArgb(35, 110, 160);
            btnUpdate.MouseLeave += (s, e) => btnUpdate.BackColor = Color.FromArgb(52, 152, 219);

            btnExit.BackColor = Color.FromArgb(231, 76, 60);               
            btnExit.MouseEnter += (s, e) => btnExit.BackColor = Color.FromArgb(176, 52, 40);      
            btnExit.MouseLeave += (s, e) => btnExit.BackColor = Color.FromArgb(231, 76, 60);
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);


        private void ChangeAccount()
        {
            Account user = AccountService.Instance.User;

            textBoxUsername.Text = user.UserName;
            textBoxDisplayName.Text = user.DisplayName;
        }

        private void RoundPanel(Panel panel, int radius)
        {
            panel.Region = Region.FromHrgn(CreateRoundRectRgn(
                0, 0, panel.Width, panel.Height, radius, radius));
        }

        private void StylePanels()
        {
            int radius = 15; 

            foreach (Panel pnl in new[] { panel1, panel2, panel4, panel5, panel6, panel7, panel8, panel9 })
            {
                RoundPanel(pnl, radius);              
            }
        }

        private void LoadAccountImageToButtonUpload()
        {
            Account user = AccountService.Instance.User;

            ImageService.Instance.LoadAccountImageToButton(user, btnUploadAccount);

            _fileName = user.Image;
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

            if (isClose && updatedUser != null)
            {
                _tableManager.accountToolStripDropdown.Text = $"Account ({updatedUser.DisplayName})";

                Image img = ImageService.Instance.LoadAccountImage(updatedUser);
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
