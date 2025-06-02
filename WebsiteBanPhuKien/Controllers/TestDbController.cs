<<<<<<< Updated upstream
<<<<<<< Updated upstream
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebsiteBanPhuKien.Models;

namespace WebsiteBanPhuKien.Controllers
{
    public class TestDbController : Controller
    {
        private readonly AppDbContext _context;

        public TestDbController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var result = new
                {
                    PhuKienCount = await _context.PhuKiens.CountAsync(),
                    LoaiPhuKienCount = await _context.LoaiPhuKiens.CountAsync(),
                    HangSanXuatCount = await _context.HangSanXuats.CountAsync(),
                    PhuKiens = await _context.PhuKiens.ToListAsync(),
                    LoaiPhuKiens = await _context.LoaiPhuKiens.ToListAsync(),
                    HangSanXuats = await _context.HangSanXuats.ToListAsync()
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        public async Task<IActionResult> CreateSampleData()
        {
            try
            {
                // Tạo loại phụ kiện mẫu nếu chưa có
                if (!await _context.LoaiPhuKiens.AnyAsync())
                {
                    _context.LoaiPhuKiens.Add(new LoaiPhuKien { TenLoai = "Ốp lưng", MoTa = "Ốp lưng điện thoại" });
                    _context.LoaiPhuKiens.Add(new LoaiPhuKien { TenLoai = "Tai nghe", MoTa = "Tai nghe có dây và không dây" });
                    await _context.SaveChangesAsync();
                }

                // Tạo hãng sản xuất mẫu nếu chưa có
                if (!await _context.HangSanXuats.AnyAsync())
                {
                    _context.HangSanXuats.Add(new HangSanXuat { TenHang = "Apple", MoTa = "Phụ kiện Apple" });
                    _context.HangSanXuats.Add(new HangSanXuat { TenHang = "Samsung", MoTa = "Phụ kiện Samsung" });
                    await _context.SaveChangesAsync();
                }

                // Tạo sản phẩm mẫu
                var loai1 = await _context.LoaiPhuKiens.FirstOrDefaultAsync(l => l.TenLoai == "Ốp lưng");
                var loai2 = await _context.LoaiPhuKiens.FirstOrDefaultAsync(l => l.TenLoai == "Tai nghe");
                var hang1 = await _context.HangSanXuats.FirstOrDefaultAsync(h => h.TenHang == "Apple");
                var hang2 = await _context.HangSanXuats.FirstOrDefaultAsync(h => h.TenHang == "Samsung");

                if (loai1 != null && hang1 != null)
                {
                    _context.PhuKiens.Add(new PhuKien
                    {
                        TenPhuKien = "Ốp lưng iPhone 15",
                        Gia = 250000,
                        MoTa = "Ốp lưng chính hãng cho iPhone 15",
                        HinhAnh = "/img/oplungtrong 16.jpg",
                        SoLuong = 50,
                        MaLoai = loai1.MaLoai,
                        MaHang = hang1.MaHang,
                        CreatedAt = DateTime.Now
                    });
                }

                if (loai2 != null && hang1 != null)
                {
                    _context.PhuKiens.Add(new PhuKien
                    {
                        TenPhuKien = "Tai nghe AirPods Pro",
                        Gia = 5500000,
                        MoTa = "Tai nghe không dây AirPods Pro chính hãng",
                        HinhAnh = "/img/airpods.jpg",
                        SoLuong = 20,
                        MaLoai = loai2.MaLoai,
                        MaHang = hang1.MaHang,
                        CreatedAt = DateTime.Now
                    });
                }

                if (loai1 != null && hang2 != null)
                {
                    _context.PhuKiens.Add(new PhuKien
                    {
                        TenPhuKien = "Ốp lưng Samsung Galaxy S23",
                        Gia = 200000,
                        MoTa = "Ốp lưng chính hãng cho Samsung Galaxy S23",
                        HinhAnh = "/img/oplungdeos23.jpg",
                        SoLuong = 30,
                        MaLoai = loai1.MaLoai,
                        MaHang = hang2.MaHang,
                        CreatedAt = DateTime.Now
                    });
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã tạo dữ liệu mẫu thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}
=======
// File này chỉ dùng để kiểm tra cơ sở dữ liệu trong quá trình phát triển
// Có thể xóa an toàn trong môi trường production
>>>>>>> Stashed changes
=======
// File này chỉ dùng để kiểm tra cơ sở dữ liệu trong quá trình phát triển
// Có thể xóa an toàn trong môi trường production
>>>>>>> Stashed changes
