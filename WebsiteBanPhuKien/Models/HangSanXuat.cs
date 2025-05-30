using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteBanPhuKien.Models
{
    [Table("Hang")]
    public class HangSanXuat
    {
        [Key]
        public int MaHang { get; set; }
        public string TenHang { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public ICollection<PhuKien> PhuKiens { get; set; } = new List<PhuKien>();
    }
}