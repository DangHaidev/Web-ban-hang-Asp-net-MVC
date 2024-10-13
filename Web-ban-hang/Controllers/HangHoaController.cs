using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_ban_hang.Data;
using Web_ban_hang.ViewModels;

namespace Web_ban_hang.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly HDangShopContext db;

        public HangHoaController(HDangShopContext context) => db = context;
        public IActionResult Index(int? loai,int? sortList)
        {
            ViewBag.Loai = loai;
            var hangHoas = db.HangHoas.AsQueryable();

            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }
            switch (sortList)
            {
                case 1:
                    hangHoas = hangHoas.OrderBy(p => p.DonGia);
                    break;
                case 2:
                    hangHoas = hangHoas.OrderByDescending(p => p.DonGia);
                    break;
            }
                    var result = hangHoas.Select( p => new HangHoaVM
            {
                MaHH = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                 TenLoai = p.MaLoaiNavigation.TenLoai

            });
            return View(result);
        }
        public IActionResult SortByPrice(int? loai)
        {
            var hangHoas = db.HangHoas.AsQueryable();

            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }
            
            var result = hangHoas.Select(p => new HangHoaVM
            {               
                MaHH = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai

            }).OrderBy(p => p.DonGia);
            return View(result);
        }

        public IActionResult Search(string? query)
        {
            var hangHoas = db.HangHoas.AsQueryable();

            if (query != null)
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }

            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHH = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai

            });
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
                MaHH =data.MaHh,
                TenHH =data.TenHh,
                DonGia = data.DonGia ?? 0,
                ChiTiet = data.MoTa ?? string.Empty,
                Hinh = data.Hinh ?? string.Empty,
                MoTaNgan = data.MoTaDonVi ?? string.Empty,  
                TenLoai = data.MaLoaiNavigation.TenLoai,
                SoLuongTon = 10,
                DiemDanhGia = 5
            };
            return View(result);
        }
    }
}
