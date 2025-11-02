using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormApp.Forms;
using WinFormApp.Models;

namespace WinFormApp.Services
{
    internal class ImageService
    {
        private static ImageService instance;

        public static ImageService Instance
        {
            get
            {
                if (instance == null) instance = new ImageService();
                return ImageService.instance;
            }
            private set { ImageService.instance = value; }
        }

        public void LoadAccountImageToButton(Account account, Button button)
        {
            Image profileImage = LoadAccountImage(account);

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

        public string UploadAccountImageDialog(OpenFileDialog dialog)
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

                    return fileName;
                }
                catch (Exception ex)
                {
                    Alert.ShowAlert($"Image upload failed: {ex.Message}", Alert.AlertType.Error);
                    return null;
                }
            }

            return null;
        }

        public void UpdateButtonImage(string filename, Button button)
        {
            string imageFolder = Path.Combine(Application.StartupPath, "Image");
            string imagePath = Path.Combine(imageFolder, filename);

            if (File.Exists(imagePath))
            {
                using (var original = Image.FromFile(imagePath))
                {
                    button.BackgroundImage = new Bitmap(original);
                }

                button.BackgroundImageLayout = ImageLayout.Zoom;
                button.Text = "";
            }
            else
            {
                Alert.ShowAlert($"Image file not found: {imagePath}", Alert.AlertType.Error);
            }
        }

        public Image LoadAccountImage(Account account)
        {
            try
            {
                string imageFolder = Path.Combine(Application.StartupPath, "Image");
                string imageFile = account?.Image ?? "default.png";
                string imagePath = Path.Combine(imageFolder, imageFile);

                if (!File.Exists(imagePath))
                    imagePath = Path.Combine(imageFolder, "default.png");

                if (!File.Exists(imagePath))
                    return null;

                return Image.FromFile(imagePath); 
            }
            catch
            {
                return null;
            }
        }

        private async Task<List<string>> GetImageList()
        {
            using (var context = new CoffeeShopContext())
            {
                return await context.Accounts.Select(a => a.Image).ToListAsync();
            }
        }

        public async Task CleanUpUnusedImages()
        {
            try
            {
                string imageFolder = Path.Combine(Application.StartupPath, "Image");

                if (!Directory.Exists(imageFolder))
                {
                    Console.WriteLine($"Image folder not found: {imageFolder}");
                    return;
                }

                List<string> imageNamesInDb = await GetImageList();

                var validNames = imageNamesInDb
                    .Where(n => !string.IsNullOrEmpty(n))
                    .Select(n => n.ToLower())
                    .ToHashSet();

                string[] files = Directory.GetFiles(imageFolder);

                foreach (string filePath in files)
                {
                    string fileName = Path.GetFileName(filePath).ToLower();

                    if (fileName == "default.png") continue;

                    if (!validNames.Contains(fileName))
                    {
                        try
                        {
                            File.Delete(filePath);
                            Console.WriteLine($"Deleted unused image: {fileName}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to delete {fileName}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Image cleanup failed: {ex.Message}");
            }
        }
    }
}
