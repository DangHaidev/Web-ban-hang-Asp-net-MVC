using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_ban_hang.Models.Entities;
using Web_ban_hang.ViewModels;

namespace Web_ban_hang.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class InvoiceController : Controller
    {
        private readonly HDangShopContext db; // Để sử dụng context


        // Constructor duy nhất kết hợp cả hai tham số
        public InvoiceController(HDangShopContext context)
        {
            db = context;
        }
        public IActionResult Invoice(int id)
        {
            var data = db.ChiTietHds.Include(p => p.MaHdNavigation).Include(p => p.MaHhNavigation).SingleOrDefault(p => p.MaCt == id);

            var result = new InvoiceVM
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
            };
            return View(result);
        }

        [HttpPost]
        public IActionResult Invoice(InvoiceVM model)
        {
            var data = db.ChiTietHds.Include(p => p.MaHdNavigation).SingleOrDefault(p => p.MaCt == model.MaCT);

            //data.MaHdNavigation.HoTen = model.HoTen;
            data.SoLuong = model.SoLuong;
            data.DonGia = model.DonGia;
            data.MaHdNavigation.MaTrangThai = model.TrangThai;
            data.MaHdNavigation.GhiChu = model.GhiChu;

            db.SaveChanges();

            return RedirectToAction("InvoiceDetail", "DataBoard");
        }

        public IActionResult ConfirmInvoice(int MaHD)
        {
            var data = db.HoaDons.SingleOrDefault(p => p.MaHd == MaHD);
            if (data.MaTrangThai == 0)
            {
                data.MaTrangThai = 1;
                TempData["Message"] = "Xac nhan don hang thanh cong!";

            }
            else
            {
                data.MaTrangThai = 0;
                TempData["Message"] = "Hoan don hang thanh cong!";


            }
            db.SaveChanges();
            return RedirectToAction("InvoiceDetail", "DataBoard");

        }
    }
}
