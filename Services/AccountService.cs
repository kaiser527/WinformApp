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
                Account user = await context.Accounts.FindAsync(username);

                if (user == null) return new AccountDTO(false, null);
                else
                {
                    bool success = user.PassWord == password;

                    if (success) User = user;

                    return new AccountDTO(success, User);
                }
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
    }
}
