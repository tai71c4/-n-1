namespace WebsiteBanPhuKien.Models.ViewModels
{
    public class DonHangViewModel
    {
        public DonHang DonHang { get; set; } = null!;
        public List<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
        public ApplicationUser KhachHang { get; set; } = null!;
    }
}