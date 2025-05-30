using System.ComponentModel.DataAnnotations;

namespace WebsiteBanPhuKien.Models
{
    public class LoaiPhuKien
    {
        [Key]
        public int MaLoai { get; set; }
        public string TenLoai { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public ICollection<PhuKien> PhuKiens { get; set; } = new List<PhuKien>();
    }
}