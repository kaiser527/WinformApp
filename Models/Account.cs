using System.ComponentModel.DataAnnotations;

namespace WinFormApp.Models
{
    public class Account
    {
        [Key]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(100)] 
        public string DisplayName { get; set; } = "Displayname";

        [Required]
        [MaxLength(1000)] 
        public string PassWord { get; set; } = "123456";
        public int Type { get; set; } = 0;
    }
}
