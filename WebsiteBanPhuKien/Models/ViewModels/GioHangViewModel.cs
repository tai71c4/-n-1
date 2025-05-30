using System.Collections.Generic;
using System.Linq;

namespace WebsiteBanPhuKien.Models.ViewModels
{
    public class GioHangViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        
        public decimal TongTien => Items.Sum(i => i.DonGia * i.SoLuong);
    }

    public class CartItemViewModel
    {
        public int MaPhuKien { get; set; }
        public string TenPhuKien { get; set; } = string.Empty;
        public string HinhAnh { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
    }
}