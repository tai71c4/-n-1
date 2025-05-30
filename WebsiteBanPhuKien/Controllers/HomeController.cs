using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebsiteBanPhuKien.Models;

namespace WebsiteBanPhuKien.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Lấy sản phẩm mới
                var sanPhamMoi = await _context.PhuKiens
                    .Include(p => p.LoaiPhuKien)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(8)
                    .ToListAsync();

                // Lấy tin tức mới
                var tinTucMoi = await _context.TinTucs
                    .OrderByDescending(t => t.NgayDang)
                    .Take(3)
                    .ToListAsync();

                ViewBag.SanPhamMoi = sanPhamMoi;
                ViewBag.TinTucMoi = tinTucMoi;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu cho trang chủ");
                // Không làm gì, để ViewBag là null và view sẽ xử lý
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}