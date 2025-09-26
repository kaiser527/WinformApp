using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinFormApp.Models
{
    public class Bill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdTable { get; set; }

        [Required]
        public DateTime DateCheckIn { get; set; } = DateTime.Now;

        public DateTime? DateCheckOut { get; set; }

        [Required]
        public int Status { get; set; } = 0;

        [Required]
        public float Discount { get; set; } = 0;

        // Navigation property
        [ForeignKey("IdTable")]
        public TableFood Table { get; set; }

        public ICollection<BillInfo> BillInfos { get; set; }
    }
}
