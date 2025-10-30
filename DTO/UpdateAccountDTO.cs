namespace WinFormApp.DTO
{
    internal class UpdateAccountDTO
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string PassWord { get; set; }
        public string NewPassWord { get; set; }
        public string ConfirmPassWord { get; set; }
        public string Image { get; set; }

        public UpdateAccountDTO(string username, string displayname, string password, string newPassword, string confirmPassword, string image)
        {
            UserName = username;    
            DisplayName = displayname;
            PassWord = password;
            NewPassWord = newPassword;
            ConfirmPassWord = confirmPassword;
            Image = image;
        }
    }
}
