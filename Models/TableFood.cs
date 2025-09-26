using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WinFormApp.Models
{
    public class TableFood
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "Untitled";

        [Required]
        public string Status { get; set; } = "Empty";

        // Navigation
        public ICollection<Bill> Bills { get; set; }
    }
}
