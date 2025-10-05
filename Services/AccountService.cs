using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

                bool success = user.PassWord == password;

                if (success) User = user;

                return new AccountDTO(success, User);
            }
        }

        public async Task<(bool, Account)> UpdateAccount(UpdateAccountDTO updateAccountDTO)
        {
            using (var context = new CoffeeShopContext())
            {
                bool isClose = true;

                Account user = await context.Accounts.FindAsync(updateAccountDTO.UserName);

                if (user == null)
                {
                    MessageBox.Show("User not exist", "Update failed");
                    isClose = false;
                }

                if (!user.PassWord.Equals(updateAccountDTO.PassWord))
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
                    user.PassWord = updateAccountDTO.ConfirmPassWord;

                    await context.SaveChangesAsync();

                    User = user;
                }

                return (isClose, User);
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

                if (updateAccount.Role.Name.Equals("Admin") || updateAccount.Role.Name.Equals("Tester"))
                {
                    MessageBox.Show("Cannot delete admin or tester account", "Delete failed");
                    return;
                }

                context.Accounts.Remove(updateAccount);

                await context.SaveChangesAsync();   
            }
        }
    }
}
