using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WinFormApp.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public ICollection<Account> Accounts { get; set; } = new HashSet<Account>();

        public ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();
    }
}
