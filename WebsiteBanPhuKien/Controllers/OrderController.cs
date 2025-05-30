using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebsiteBanPhuKien.Models;
using WebsiteBanPhuKien.Models.ViewModels;

namespace WebsiteBanPhuKien.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> LichSu()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var donHangs = await _context.DonHangs
                .Where(d => d.UserId == user.Id)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();

            return View(donHangs);
        }

        public async Task<IActionResult> ChiTiet(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var donHang = await _context.DonHangs
                .FirstOrDefaultAsync(d => d.MaDon == id && d.UserId == user.Id);

            if (donHang == null)
            {
                return NotFound();
            }

            var chiTietDonHangs = await _context.ChiTietDonHangs
                .Include(c => c.PhuKien)
                .Where(c => c.MaDon == id)
                .ToListAsync();

            var viewModel = new DonHangViewModel
            {
                DonHang = donHang,
                ChiTietDonHangs = chiTietDonHangs,
                KhachHang = user
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDonHang(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var donHang = await _context.DonHangs
                .FirstOrDefaultAsync(d => d.MaDon == id && d.UserId == user.Id);

            if (donHang == null)
            {
                return NotFound();
            }

            if (donHang.TrangThai == "Chờ xác nhận")
            {
                donHang.TrangThai = "Đã hủy";
                donHang.UpdatedAt = DateTime.Now;
                donHang.UpdatedBy = user.Id;

                // Hoàn lại số lượng sản phẩm
                var chiTietDonHangs = await _context.ChiTietDonHangs
                    .Where(c => c.MaDon == id)
                    .ToListAsync();

                foreach (var chiTiet in chiTietDonHangs)
                {
                    var phuKien = await _context.PhuKiens.FindAsync(chiTiet.MaPhuKien);
                    if (phuKien != null)
                    {
                        phuKien.SoLuong += chiTiet.SoLuong;
                        phuKien.UpdatedAt = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể hủy đơn hàng ở trạng thái hiện tại.";
            }

            return RedirectToAction(nameof(ChiTiet), new { id });
        }
    }
}