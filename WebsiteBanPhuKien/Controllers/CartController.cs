using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebsiteBanPhuKien.Models;
using WebsiteBanPhuKien.Models.ViewModels;

namespace WebsiteBanPhuKien.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var cartItems = await _context.GioHangs
                .Include(g => g.PhuKien)
                .Where(g => g.UserId == user.Id)
                .ToListAsync();

            var viewModel = new GioHangViewModel
            {
                Items = cartItems.Select(item => new CartItemViewModel
                {
                    MaPhuKien = item.MaPhuKien,
                    TenPhuKien = item.PhuKien.TenPhuKien,
                    HinhAnh = item.PhuKien.HinhAnh,
                    DonGia = item.PhuKien.Gia,
                    SoLuong = item.SoLuong
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ThemVaoGio(int maPhuKien, int soLuong = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var phuKien = await _context.PhuKiens.FindAsync(maPhuKien);
            if (phuKien == null)
            {
                return NotFound();
            }

            var gioHang = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.UserId == user.Id && g.MaPhuKien == maPhuKien);

            if (gioHang == null)
            {
                gioHang = new GioHang
                {
                    UserId = user.Id,
                    MaPhuKien = maPhuKien,
                    SoLuong = soLuong
                };
                _context.GioHangs.Add(gioHang);
            }
            else
            {
                gioHang.SoLuong += soLuong;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Sản phẩm đã được thêm vào giỏ hàng.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatSoLuong(int maPhuKien, int soLuong)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var gioHang = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.UserId == user.Id && g.MaPhuKien == maPhuKien);

            if (gioHang == null)
            {
                return NotFound();
            }

            if (soLuong <= 0)
            {
                _context.GioHangs.Remove(gioHang);
            }
            else
            {
                gioHang.SoLuong = soLuong;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> XoaKhoiGio(int maPhuKien)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var gioHang = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.UserId == user.Id && g.MaPhuKien == maPhuKien);

            if (gioHang != null)
            {
                _context.GioHangs.Remove(gioHang);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Sản phẩm đã được xóa khỏi giỏ hàng.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ThanhToan()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var cartItems = await _context.GioHangs
                .Include(g => g.PhuKien)
                .Where(g => g.UserId == user.Id)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new GioHangViewModel
            {
                Items = cartItems.Select(item => new CartItemViewModel
                {
                    MaPhuKien = item.MaPhuKien,
                    TenPhuKien = item.PhuKien.TenPhuKien,
                    HinhAnh = item.PhuKien.HinhAnh,
                    DonGia = item.PhuKien.Gia,
                    SoLuong = item.SoLuong
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanDatHang()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var cartItems = await _context.GioHangs
                .Include(g => g.PhuKien)
                .Where(g => g.UserId == user.Id)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var donHang = new DonHang
                {
                    UserId = user.Id,
                    NgayDat = DateTime.Now,
                    TongTien = cartItems.Sum(item => item.PhuKien.Gia * item.SoLuong),
                    TrangThai = "Chờ xác nhận"
                };

                _context.DonHangs.Add(donHang);
                await _context.SaveChangesAsync();

                foreach (var item in cartItems)
                {
                    var chiTiet = new ChiTietDonHang
                    {
                        MaDon = donHang.MaDon,
                        MaPhuKien = item.MaPhuKien,
                        SoLuong = item.SoLuong,
                        DonGia = item.PhuKien.Gia
                    };
                    _context.ChiTietDonHangs.Add(chiTiet);

                    // Cập nhật số lượng sản phẩm
                    var phuKien = await _context.PhuKiens.FindAsync(item.MaPhuKien);
                    if (phuKien != null)
                    {
                        phuKien.SoLuong -= item.SoLuong;
                        phuKien.UpdatedAt = DateTime.Now;
                    }
                }

                // Xóa giỏ hàng
                _context.GioHangs.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                TempData["SuccessMessage"] = "Đặt hàng thành công!";
                return RedirectToAction("ChiTiet", "Order", new { id = donHang.MaDon });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi đặt hàng. Vui lòng thử lại sau.";
                return RedirectToAction(nameof(ThanhToan));
            }
        }
    }
}