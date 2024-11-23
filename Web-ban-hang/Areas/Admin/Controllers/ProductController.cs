using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_ban_hang.Models.Entities;
using Web_ban_hang.Helpers;
using Web_ban_hang.ViewModels;

namespace Web_ban_hang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly HDangShopContext db;

        public ProductController(HDangShopContext context) => db = context;
        public IActionResult Index()
        {
            var data = db.HangHoas
               .Include(p => p.MaLoaiNavigation).Include(h => h.MaNccNavigation).ToList();

            var result = data.Select(item => new ChiTietHangHoaVM
            {
                MaHH = item.MaHh,
                TenHH = item.TenHh,
                DonGia = item.DonGia ?? 0,
                ChiTiet = item.MoTa ?? string.Empty,
                Hinh = item.Hinh ?? string.Empty,
                MoTaNgan = item.MoTaDonVi ?? string.Empty,
                TenLoai = item.MaLoaiNavigation.TenLoai,
                SoLuongTon = 10,
                DiemDanhGia = 5
            }).ToList().OrderBy(p => p.TenLoai);
            return View(result);
        }

        public IActionResult Detail(int id)
        {
            var data = db.HangHoas
                 .Include(p => p.MaLoaiNavigation)
                 .SingleOrDefault(p => p.MaHh == id);
            if (data == null)
            {
                TempData["Message"] = "Không thấy sản phẩm có mã";
                return Redirect("/404");
            }
            var result = new ChiTietHangHoaVM
            {
                MaHH = data.MaHh,
                TenHH = data.TenHh,
                DonGia = data.DonGia ?? 0,
                ChiTiet = data.MoTa ?? string.Empty,
                Hinh = data.Hinh ?? string.Empty,
                MoTaNgan = data.MoTaDonVi ?? string.Empty,
                TenLoai = data.MaLoaiNavigation.TenLoai,
                SoLuongTon = 10,
                DiemDanhGia = 5,
                MaLoai = data.MaLoai,

            };

            ViewBag.DanhSachLoai = db.Loais.Select(l => new { l.MaLoai, l.TenLoai }).ToList();
            return View(result);
        }
        [HttpPost]
        public IActionResult Detail(ChiTietHangHoaVM model, IFormFile? Hinh)
        {
			//if (!ModelState.IsValid) return RedirectToAction("Index");

			var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == model.MaHH);

            hangHoa.TenHh = model.TenHH;
            hangHoa.DonGia = model.DonGia;
            hangHoa.MoTa = model.ChiTiet;


			if (Hinh != null)
			{
				hangHoa.Hinh = Util.UploadHinh(Hinh, "HangHoa");
			}

            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
