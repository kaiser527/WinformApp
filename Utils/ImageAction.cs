using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WinFormApp.Models;
using WinFormApp.Services;

namespace WinFormApp.Helpers
{
    internal class ImageAction
    {
        public static void LoadAccountImageToButton(Account account, Button button, int size = 84)
        {
            Image profileImage = AccountService.Instance.LoadAccountImage(account, size);

            if (profileImage != null)
            {
                button.BackgroundImage = profileImage;
                button.BackgroundImageLayout = ImageLayout.Zoom;
                button.Text = "";
            }
            else
            {
                button.Text = "+";
                button.BackgroundImage = null;
            }
        }

        public static string UploadAccountImageDialog(OpenFileDialog dialog)
        {
            dialog.Title = "Select Profile Image";
            dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = dialog.FileName;
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

        public static void UpdateButtonImage(string filename, Button button, int size = 84)
        {
            string imageFolder = Path.Combine(Application.StartupPath, "Image");
            string imagePath = Path.Combine(imageFolder, filename);

            if (File.Exists(imagePath))
            {
                using (var original = Image.FromFile(imagePath))
                {
                    button.BackgroundImage = new Bitmap(original, new Size(size, size));
                }

                button.BackgroundImageLayout = ImageLayout.Zoom;
                button.Text = "";

                MessageBox.Show($"Image '{filename}' uploaded successfully!", "Success");
            }
            else
            {
                MessageBox.Show($"Image file not found: {imagePath}", "Error");
            }
        }
    }
}
