using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinFormApp.Models
{
    public class BillInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdBill { get; set; }

        [Required]
        public int IdFood { get; set; }

        [Required]
        public int Count { get; set; } = 0;

        // Navigation properties
        [ForeignKey("IdBill")]
        public Bill Bill { get; set; }

        [ForeignKey("IdFood")]
        public Food Food { get; set; }
    }
}
