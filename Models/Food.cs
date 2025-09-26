using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinFormApp.Models
{
    public class Food
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "Untitled";

        [Required]
        public int IdCategory { get; set; }

        [Required]
        public double Price { get; set; } = 0;

        // Navigation
        [ForeignKey("IdCategory")]
        public FoodCategory Category { get; set; }
        public ICollection<BillInfo> BillInfos { get; set; }
    }
}
