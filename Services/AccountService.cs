using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WinFormApp.DTO;
using WinFormApp.Forms;
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
                    Alert.ShowAlert("Account not exist", Alert.AlertType.Error);
                    isClose = false;
                }

                bool success = BCrypt.Net.BCrypt.Verify(updateAccountDTO.PassWord, user.PassWord);

                if (!success)
                {
                    Alert.ShowAlert("Password incorrect", Alert.AlertType.Error);
                    isClose = false;
                }

                if (!updateAccountDTO.NewPassWord.Equals(updateAccountDTO.ConfirmPassWord))
                {
                    Alert.ShowAlert("Password and confirm password not match", Alert.AlertType.Error);
                    isClose = false;
                }

                if (isClose)
                {
                    user.DisplayName = updateAccountDTO.DisplayName;
                    user.PassWord = BCrypt.Net.BCrypt.HashPassword(updateAccountDTO.ConfirmPassWord);
                    user.Image = updateAccountDTO.Image;

                    await context.SaveChangesAsync();
                }

                return (isClose, user);
            }
        }

        public async Task<PaginatedResult<Account>> GetListAccount(
            int pageSize = 100,
            int pageNumber = 1,
            string name = null)
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

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PaginatedResult<Account>
                {
                    Items = items,
                    TotalCount = totalCount,
                    TotalPages = totalPages
                };
            }
        }

        public async Task InsertAccount(Account account)
        {
            using (var context = new CoffeeShopContext())
            {
                bool isExist = await context.Accounts.AnyAsync(a => a.UserName == account.UserName);

                if (isExist)
                {
                    Alert.ShowAlert("Account is already exist", Alert.AlertType.Error);
                    return;
                }

                bool isExistRole = await context.Roles.AnyAsync(r => r.Id == account.IdRole);

                if (!isExistRole)
                {
                    Alert.ShowAlert("Role is not exist", Alert.AlertType.Error);
                    return;
                }

                context.Accounts.Add(account);

                await context.SaveChangesAsync();   
            }
        }

        public async Task<Account> UpdateAccount(Account account)
        {
            using (var context = new CoffeeShopContext())
            {
                Account updateAccount = await context.Accounts
                    .Include(a => a.Role)
                    .FirstOrDefaultAsync(a => a.UserName == account.UserName);

                if(updateAccount == null)
                {
                    Alert.ShowAlert("Account is not exist", Alert.AlertType.Error);
                    return updateAccount;
                }

                bool isExistRole = await context.Roles.AnyAsync(r => r.Id == account.IdRole);

                if (!isExistRole)
                {
                    Alert.ShowAlert("Role is not exist", Alert.AlertType.Error);
                    return updateAccount;
                }

                updateAccount.DisplayName = account.DisplayName;
                updateAccount.Image = account.Image;
                updateAccount.IdRole = account.IdRole;

                await context.SaveChangesAsync();

                return updateAccount;
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
                    Alert.ShowAlert("Account is not exist", Alert.AlertType.Error);
                    return;
                }

                if (User != null && updateAccount.UserName == User.UserName)
                {
                    Alert.ShowAlert("Cannot delete yourself", Alert.AlertType.Error);
                    return;
                }

                if (updateAccount.Role.Name.Equals("Admin"))
                {
                    Alert.ShowAlert("Cannot delete admin account", Alert.AlertType.Error);
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
                    Alert.ShowAlert("Account is not exist", Alert.AlertType.Error);
                    return;
                }

                if (resetAccount.Role.Name.Equals("Admin"))
                {
                    Alert.ShowAlert("Cannot reset admin account", Alert.AlertType.Error);
                    return;
                }

                resetAccount.PassWord = BCrypt.Net.BCrypt.HashPassword("123456");

                await context.SaveChangesAsync();
            }
        }

        public async Task<Account> GetSingleAccount(string username)
        {
            using (var context = new CoffeeShopContext())
            {
                var account = await context.Accounts
                    .Include(a => a.Role)
                    .FirstOrDefaultAsync(a => a.UserName == username);

                if (account == null)
                {
                    Alert.ShowAlert("Account is not exist", Alert.AlertType.Error);
                    return new Account();
                }

                return account;
            }
        }
    }
}
