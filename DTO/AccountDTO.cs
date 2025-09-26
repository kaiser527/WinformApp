using WinFormApp.Models;

namespace WinFormApp.DTO
{
    internal class AccountDTO
    {
        public bool IsAuthenticated { get; set; }
        public Account User { get; set; }

        public AccountDTO(bool isAuthenticated, Account user) 
        {
            IsAuthenticated = isAuthenticated;
            User = user;
        }
    }
}
