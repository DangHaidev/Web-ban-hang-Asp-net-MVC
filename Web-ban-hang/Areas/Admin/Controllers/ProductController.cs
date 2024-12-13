using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_ban_hang.Models.Entities;
using Web_ban_hang.Helpers;
using Web_ban_hang.ViewModels;
using X.PagedList.Extensions;

namespace Web_ban_hang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly HDangShopContext db;

        public ProductController(HDangShopContext context) => db = context;
            public IActionResult Index(int? page)
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
                DiemDanhGia = 5,
                TrangThai = item.TrangThai,
                
            }).ToList().OrderBy(p => p.TenLoai);

            ViewBag.TotalProduct = db.HangHoas.Count();
            // Thực hiện phân trang
            int pageSize = 10; // Số sản phẩm trên mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là trang 1
            var pagedResult = result.ToPagedList(pageNumber, pageSize);
            ViewBag.page = page;
            return View(pagedResult);
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
                TrangThai = data.TrangThai,
                MaNcc = data.MaNcc,
                GiamGia = data.GiamGia,
                NgaySx = data.NgaySx,

            };

            ViewBag.DanhSachLoai = db.Loais.Select(l => new { l.MaLoai, l.TenLoai }).ToList();
			ViewBag.DanhSachNCC = db.NhaCungCaps.Select(l => new { l.MaNcc, l.TenCongTy }).ToList();

			return View(result);
        }
        [HttpPost]
        public IActionResult Detail(ChiTietHangHoaVM model, IFormFile? Hinh)
        {

			var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == model.MaHH);



            hangHoa.TenHh = model.TenHH;
            hangHoa.DonGia = model.DonGia;
            hangHoa.MoTa = model.ChiTiet;
            hangHoa.GiamGia = model.GiamGia / 100;
            hangHoa.MoTaDonVi = model.MoTaNgan;
            hangHoa.MaLoai = model.MaLoai;
            hangHoa.MaNcc = model.MaNcc;
            hangHoa.NgaySx = DateTime.Now;


			if (Hinh != null)
			{
				hangHoa.Hinh = Util.UploadHinh(Hinh, "HangHoa");
			}
            TempData["Success"] = "Cập nhật sản phẩm thành công!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult HideProduct(int id)
        {
            var pro = db.HangHoas.FirstOrDefault(p => p.MaHh == id);
            if (pro.TrangThai == true)
            {
                pro.TrangThai = false;
            }
            else
            {
                pro.TrangThai = true;
            }

            TempData["Success"] = "Cập nhật sản phẩm thành công!";
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult AddProduct() {
            ViewBag.DanhSachLoai = db.Loais.Select(l => new { l.MaLoai, l.TenLoai }).ToList();
			ViewBag.DanhSachNCC = db.NhaCungCaps.Select(l => new { l.MaNcc, l.TenCongTy }).ToList();

			return View(); }
        [HttpPost]
        public IActionResult AddProduct(ChiTietHangHoaVM model, IFormFile? Hinh) 
        {
			ViewBag.DanhSachLoai = db.Loais.Select(l => new { l.MaLoai, l.TenLoai }).ToList();
			ViewBag.DanhSachNCC = db.NhaCungCaps.Select(l => new { l.MaNcc, l.TenCongTy }).ToList();

            if (ModelState.IsValid)
            {

                var product = new HangHoa
                {

                    TenHh = model.TenHH,
                    MoTaDonVi = model.MoTaNgan,
                    DonGia = model.DonGia,
                    MoTa = model.ChiTiet,
                    MaLoai = model.MaLoai,
                    TrangThai = model.TrangThai,
                    MaNcc = model.MaNcc,
                    NgaySx = DateTime.Now,
                    SoLanXem = 0,
                    GiamGia = model.GiamGia / 100,
                };


                if (Hinh != null)
                {
                    product.Hinh = Util.UploadHinh(Hinh, "HangHoa");
                }
                TempData["Success"] = "Thêm sản phẩm thành công!";

                db.HangHoas.Add(product);
                db.SaveChanges();
            }
            else
            {
                TempData["Error"] = "Có lỗi trong quá trình thêm sản phẩm";
            }
            return RedirectToAction("Index");
        }
    }
}
