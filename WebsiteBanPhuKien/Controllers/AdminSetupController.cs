using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebsiteBanPhuKien.Models;
using WebsiteBanPhuKien.Models.ViewModels;

namespace WebsiteBanPhuKien.Controllers
{
    public class AdminSetupController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public AdminSetupController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            // Kiểm tra xem đã có admin chưa
            var adminExists = await _userManager.GetUsersInRoleAsync("Admin");
            if (adminExists.Count > 0)
            {
                return View("AdminExists");
            }

            return View(new AdminSetupViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AdminSetupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra xem đã có admin chưa
            var adminExists = await _userManager.GetUsersInRoleAsync("Admin");
            if (adminExists.Count > 0)
            {
                return View("AdminExists");
            }

            // Kiểm tra mã bảo mật
            if (model.InputSecurityCode != model.SecurityCode)
            {
                ModelState.AddModelError("InputSecurityCode", "Mã bảo mật không đúng");
                return View(model);
            }

            // Tạo role Admin nếu chưa có
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            }

            // Tạo tài khoản admin
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                HoTen = model.HoTen,
                Avatar = "/images/avatars/default.jpg",
                TrangThai = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                return View("Success");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        public IActionResult AdminExists()
        {
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}