using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteBanPhuKien.Models
{
    [Table("AspNetUsers")]
    public class ApplicationUser : IdentityUser<Guid>
    {
        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        public string Avatar { get; set; } = "/images/avatars/default.jpg";
        public bool TrangThai { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();
        public ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
        public ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
        public ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();
    }
}