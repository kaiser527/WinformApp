using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WinFormApp.Models
{
    public class FoodCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "Untitled";

        // Navigation
        public ICollection<Food> Foods { get; set; }
    }
}
