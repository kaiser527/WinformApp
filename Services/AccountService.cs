using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormApp.DTO;
using WinFormApp.Models;

namespace WinFormApp.Services
{
    internal class AccountService
    {
        private static AccountService instance;

        public static AccountService Instance
        {
            get
            {
                if (instance == null) instance = new AccountService();
                return AccountService.instance;
            }
            private set { AccountService.instance = value; }
        }

        private AccountService() { }

        public Account User;

        public async Task<AccountDTO> Login(string username, string password)
        {
            using (var context = new CoffeeShopContext())
            {
                Account user = await context.Accounts
                    .Include(a => a.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(a => a.UserName == username);

                if (user == null) return new AccountDTO(false, null);

                bool success = BCrypt.Net.BCrypt.Verify(password, user.PassWord);

                if (success) User = user;

                return new AccountDTO(success, success ? user : null);
            }
        }

        public async Task<(bool, Account)> UpdateAccount(UpdateAccountDTO updateAccountDTO)
        {
            using (var context = new CoffeeShopContext())
            {
                bool isClose = true;

                Account user = await context.Accounts
                    .Include(a => a.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(a => a.UserName == updateAccountDTO.UserName);

                if (user == null)
                {
                    MessageBox.Show("User not exist", "Update failed");
                    isClose = false;
                }

                bool success = BCrypt.Net.BCrypt.Verify(updateAccountDTO.PassWord, user.PassWord);

                if (!success)
                {
                    MessageBox.Show("Password incorrect", "Update failed");
                    isClose = false;
                }

                if (!updateAccountDTO.NewPassWord.Equals(updateAccountDTO.ConfirmPassWord))
                {
                    MessageBox.Show("Password and confirm password not match", "Update failed");
                    isClose = false;
                }

                if (isClose)
                {
                    user.DisplayName = updateAccountDTO.DisplayName;
                    user.PassWord = BCrypt.Net.BCrypt.HashPassword(updateAccountDTO.ConfirmPassWord);
                    user.Image = updateAccountDTO.Inage;

                    await context.SaveChangesAsync();
                }

                return (isClose, user);
            }
        }

        public async Task<IEnumerable<Account>> GetListAccount(string name = null)
        {
            using (var context = new CoffeeShopContext())
            {
                var query = context.Accounts
                   .Include(a => a.Role)
                   .AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(a => 
                        a.UserName.ToLower().Contains(name.ToLower()) || 
                        a.DisplayName.ToLower().Contains(name.ToLower())
                    );
                }

                return await query.ToListAsync();
            }
        }

        public async Task InsertAccount(Account account)
        {
            using (var context = new CoffeeShopContext())
            {
                bool isExist = await context.Accounts.AnyAsync(a => a.UserName == account.UserName);

                if (isExist)
                {
                    MessageBox.Show("Account is already exist", "Insert failed");
                    return;
                }

                context.Accounts.Add(account);

                await context.SaveChangesAsync();   
            }
        }

        public async Task UpdateAccount(Account account)
        {
            using (var context = new CoffeeShopContext())
            {
                Account updateAccount = await context.Accounts
                    .Include(a => a.Role)
                    .FirstOrDefaultAsync(a => a.UserName == account.UserName);

                if(updateAccount == null)
                {
                    MessageBox.Show("Account is not exist", "Update failed");
                    return;
                }

                bool isExist = await context.Accounts
                    .AnyAsync(a => a.DisplayName == account.DisplayName && a.UserName != account.UserName);

                if (isExist)
                {
                    MessageBox.Show("Account is already exist", "Update failed");
                    return;
                }

                updateAccount.DisplayName = account.DisplayName;
                updateAccount.IdRole = account.IdRole;

                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAccount(string username)
        {
            using (var context = new CoffeeShopContext())
            {
                Account updateAccount = await context.Accounts
                   .Include(a => a.Role)
                   .FirstOrDefaultAsync(a => a.UserName == username);

                if (updateAccount == null)
                {
                    MessageBox.Show("Account is not exist", "Delete failed");
                    return;
                }

                if (updateAccount.Role.Name.Equals("Admin"))
                {
                    MessageBox.Show("Cannot delete admin account", "Delete failed");
                    return;
                }

                context.Accounts.Remove(updateAccount);

                await context.SaveChangesAsync();   
            }
        }

        public async Task ResetAccountPassword(string username)
        {
            using (var context = new CoffeeShopContext())
            {
                Account resetAccount = await context.Accounts
                  .Include(a => a.Role)
                  .FirstOrDefaultAsync(a => a.UserName == username);

                if (resetAccount == null)
                {
                    MessageBox.Show("Account is not exist", "Reset failed");
                    return;
                }

                if (resetAccount.Role.Name.Equals("Admin"))
                {
                    MessageBox.Show("Cannot reset admin account password", "Reset failed");
                    return;
                }

                resetAccount.PassWord = BCrypt.Net.BCrypt.HashPassword("123456");

                await context.SaveChangesAsync();
            }
        }

        public Image LoadAccountImage(Account account, int size = 64)
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

                using (var original = Image.FromFile(imagePath))
                {
                    return new Bitmap(original, new Size(size, size));
                }
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
