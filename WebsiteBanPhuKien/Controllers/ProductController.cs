using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebsiteBanPhuKien.Models;
using WebsiteBanPhuKien.Models.ViewModels;

namespace WebsiteBanPhuKien.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> DanhSach(int? loaiId, int? hangId, string searchString, int page = 1)
        {
            int pageSize = 9;
            var products = _context.PhuKiens
                .Include(p => p.LoaiPhuKien)
                .Include(p => p.HangSanXuat)
                .AsQueryable();

            if (loaiId.HasValue)
            {
                products = products.Where(p => p.MaLoai == loaiId);
                ViewBag.Loai = await _context.LoaiPhuKiens.FindAsync(loaiId);
            }

            if (hangId.HasValue)
            {
                products = products.Where(p => p.MaHang == hangId);
                ViewBag.Hang = await _context.HangSanXuats.FindAsync(hangId);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.TenPhuKien.Contains(searchString));
            }

            var count = await products.CountAsync();
            var items = await products
                .OrderBy(p => p.MaPhuKien) // Thêm OrderBy để tránh warning
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            ViewBag.LoaiPhuKiens = await _context.LoaiPhuKiens.ToListAsync();
            ViewBag.HangSanXuats = await _context.HangSanXuats.ToListAsync();
            ViewBag.SearchString = searchString;

            return View(items);
        }

        public async Task<IActionResult> ChiTiet(int id)
        {
            var sanPham = await _context.PhuKiens
                .Include(p => p.LoaiPhuKien)
                .Include(p => p.HangSanXuat)
                .FirstOrDefaultAsync(p => p.MaPhuKien == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            var sanPhamCungLoai = await _context.PhuKiens
                .Where(p => p.MaLoai == sanPham.MaLoai && p.MaPhuKien != id)
                .Take(4)
                .ToListAsync();

            var danhGias = await _context.DanhGias
                .Include(d => d.User)
                .Where(d => d.MaPhuKien == id)
                .ToListAsync();

            var diemTrungBinh = danhGias.Any() ? danhGias.Average(d => d.SoSao) : 0;
            var tongDanhGia = danhGias.Count;

            var viewModel = new ChiTietSanPhamViewModel
            {
                SanPham = sanPham,
                SanPhamCungLoai = sanPhamCungLoai,
                DanhGias = danhGias,
                DiemTrungBinh = diemTrungBinh,
                TongDanhGia = tongDanhGia
            };

            return View(viewModel);
        }

        public async Task<IActionResult> TheoLoai(int id)
        {
            var loai = await _context.LoaiPhuKiens.FindAsync(id);
            if (loai == null)
            {
                return NotFound();
            }

            var products = await _context.PhuKiens
                .Include(p => p.LoaiPhuKien)
                .Include(p => p.HangSanXuat)
                .Where(p => p.MaLoai == id)
                .ToListAsync();

            ViewBag.Loai = loai;
            return View(products);
        }
    }
}