using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_ban_hang.Models.Entities;
using Web_ban_hang.Services;
using Web_ban_hang.ViewModels;

namespace Web_ban_hang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DataBoardController : Controller
    {
        private readonly HDangShopContext db; // Để sử dụng context


        // Constructor duy nhất kết hợp cả hai tham số
        public DataBoardController(HDangShopContext context)
        {
            db = context;
        }


        public IActionResult Index()
        {
            var data = db.HoaDons.Include(p => p.MaKhNavigation)
               .OrderByDescending(o => o.NgayDat)
               .Take(5)
               .ToList()
               .Where(p => p.MaTrangThai == 0);

            var result = data.Select(item => new InvoiceVM
            {
                MaHD = item.MaHd,
                HoTen = item.MaKhNavigation.HoTen,
                TongTien = 100,
                NgayDat = item.NgayDat,
                TrangThai = item.MaTrangThai,
            }).ToList();


            return View(result);
        }
        public IActionResult InvoiceDetail()
        {
            //var data = db.ChiTietHds.Include(p => p.MaHdNavigation).Include(p => p.MaHhNavigation).OrderByDescending(p => p.MaHd).ToList();

            //var result = data.Select(hd => new InvoiceVM
            //{
            //    MaCT = hd.MaCt,
            //    MaHD = hd.MaHd,
            //    NgayDat = hd.MaHdNavigation.NgayDat,
            //    MaKH = hd.MaHdNavigation.MaKh,
            //    HoTen = hd.MaHdNavigation.HoTen,
            //    DiaChi = hd.MaHdNavigation.DiaChi,
            //    CachThanhToan = hd.MaHdNavigation.CachThanhToan,
            //    GhiChu = hd.MaHdNavigation.GhiChu,
            //    TrangThai = hd.MaHdNavigation.MaTrangThai,
            //    TenSP = hd.MaHhNavigation.TenHh,
            //    SoLuong = hd.SoLuong,
            //    DonGia = hd.DonGia,
            //});

            var data = db.HoaDons.Include(p => p.MaKhNavigation)
                .OrderByDescending(o => o.NgayDat)
                .ToList();

            var result = data.Select(item => new InvoiceVM
            {
                MaHD = item.MaHd,
                HoTen = item.MaKhNavigation.HoTen,
                NgayCan = item.NgayCan ?? item.NgayDat,
                NgayDat = item.NgayDat,
                TrangThai = item.MaTrangThai,
            }).ToList();

            return View(result);
        }

        public IActionResult ViewOrder(int id)
        {
            var cthd = db.ChiTietHds.Include(p => p.MaHdNavigation).Include(p => p.MaHhNavigation).Where(p => p.MaHd == id).ToList();

            ViewBag.MaHD = id;
            //ViewBag.TenKH = db.ChiTietHds.Where(p => p.MaHd == id).FirstOrDefault().MaHdNavigation.HoTen;

            var chiTiet = db.HoaDons
    .Where(p => p.MaHd == id)
    .Include(p => p.MaKhNavigation) // Đảm bảo Include quan hệ để tránh lỗi null
    .FirstOrDefault();

            if (chiTiet != null && chiTiet.MaKhNavigation != null)
            {
                ViewBag.TenKH = chiTiet.MaKhNavigation.HoTen;
            }
            else
            {
                ViewBag.TenKH = "NULL";
            }


            var result = cthd.Select(data => new InvoiceVM
            {
                MaCT = data.MaCt,
                MaHD = data.MaHd,
                NgayDat = data.MaHdNavigation.NgayDat,
                MaKH = data.MaHdNavigation.MaKh,
                HoTen = data.MaHdNavigation.HoTen,
                DiaChi = data.MaHdNavigation.DiaChi,
                CachThanhToan = data.MaHdNavigation.CachThanhToan,
                GhiChu = data.MaHdNavigation.GhiChu,
                TrangThai = data.MaHdNavigation.MaTrangThai,
                TenSP = data.MaHhNavigation.TenHh,
                SoLuong = data.SoLuong,
                DonGia = data.DonGia,
            }).ToList();



            return View(result);
        }

        [HttpPost]
        public IActionResult ViewOrder(InvoiceVM model)
        {
            var data = db.ChiTietHds.Include(p => p.MaHdNavigation).SingleOrDefault(p => p.MaCt == model.MaCT);

            //data.MaHdNavigation.HoTen = model.HoTen;
            data.SoLuong = model.SoLuong;
            data.DonGia = model.DonGia;
            data.MaHdNavigation.MaTrangThai = model.TrangThai;
            data.MaHdNavigation.GhiChu = model.GhiChu;

            db.SaveChanges();

            return RedirectToAction("Index");

        }

        public IActionResult Product()
        {
            return RedirectToAction("Index");
        }
        //public async Task<IActionResult> Product()
        //{
        //    var hDangShopContext = db.HangHoas.Include(h => h.MaLoaiNavigation).Include(h => h.MaNccNavigation);
        //    return View(await hDangShopContext.ToListAsync());
        //}
        public IActionResult Customer()
        {
            return View();
        }

    }
}
