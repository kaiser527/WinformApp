using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required]
        public int IdRole { get; set; }

        [ForeignKey("IdRole")]
        public Role Role { get; set; }
    }
}
