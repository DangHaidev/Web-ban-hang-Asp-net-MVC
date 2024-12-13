using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_ban_hang.Helpers;
using Web_ban_hang.Models.Entities;
using Web_ban_hang.Repository;
using Web_ban_hang.ViewModels;

namespace Web_ban_hang.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly HDangShopContext _context; // Để sử dụng context
        private readonly IEmailSender _emailSender;

        public CheckoutController(HDangShopContext db, IEmailSender emailSender)
        {
            _context = db;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> CreateInvoice()
        {
            string customerId = HttpContext.User.FindFirst("CustomerID")?.Value;

            var data = _context.KhachHangs.FirstOrDefault(p => p.MaKh == customerId);

            var gioHang = HttpContext.Session.Get<List<CartItem>>("MYCART");




            var hoaDon = new HoaDon
            {
                NgayDat = DateTime.Now,
                MaKh = customerId,
                DiaChi = data.DiaChi,
                CachThanhToan = "Cash",
                CachVanChuyen = "Airline",
                PhiVanChuyen = 0,
                MaTrangThai = 1,
                GhiChu = "note"

            };

            _context.HoaDons.Add(hoaDon);
            _context.SaveChanges();

            int Mahdnew = hoaDon.MaHd;

            foreach (var item in gioHang)
            {
                var orderdetails = new ChiTietHd
                {
                    MaHd = Mahdnew,
                    MaHh = item.MaHh,
                    DonGia = item.DonGia,
                    SoLuong = item.SoLuong,
                    GiamGia = 0
                };


                _context.ChiTietHds.Add(orderdetails);
                _context.SaveChanges();
            }




            HttpContext.Session.Remove("MYCART");


            var receive = User.FindFirst((ClaimTypes.Email)).Value;
            var subject = "Thư cảm ơn";
            var message = "Cảm ơn bạn đã đặt hàng của chúng tôi, đơn hàng sẽ sớm được chuẩn bị và vận chuyển đến cho bạn!!";

            await _emailSender.SendEmailAsync(receive, subject, message);

            TempData["Success"] = "Cảm ơn bạn, Đơn đã được đặt hàng thành công!";

            return RedirectToAction("index", "Home");
        }

        [Authorize]
        public IActionResult Ordering()
        {
            var MaKh = User.FindFirst("CustomerID").Value;

            var data = _context.ChiTietHds.Include(p => p.MaHdNavigation).Include(p => p.MaHhNavigation).Where(p => p.MaHdNavigation.MaKh == MaKh && p.MaHdNavigation.MaTrangThai != 3).ToList();

            var result = data.Select(item => new Order
            {
				maHD = item.MaHd,
				maCT = item.MaCt,
                MaHh = item.MaHh,
                Hinh = item.MaHhNavigation.Hinh,
                TenHh = item.MaHhNavigation.TenHh,
                SoLuong = item.SoLuong,
                DonGia = item.DonGia,
                TrangThai = item.MaHdNavigation.MaTrangThai,

            });

			return View(result);
        }
        [Authorize]
        public IActionResult Ordered()
        {
            var MaKh = User.FindFirst("CustomerID").Value;

            var data = _context.ChiTietHds.Include(p => p.MaHdNavigation).Include(p => p.MaHhNavigation).Where(p => p.MaHdNavigation.MaKh == MaKh && p.MaHdNavigation.MaTrangThai == 3).ToList();

            var result = data.Select(item => new Order
            {
                maHD = item.MaHd,
                MaHh = item.MaHh,
                Hinh = item.MaHhNavigation.Hinh,
                TenHh = item.MaHhNavigation.TenHh,
                SoLuong = item.SoLuong,
                DonGia = item.DonGia,
                TrangThai = item.MaHdNavigation.MaTrangThai,
                NgayGiao = item.MaHdNavigation.NgayGiao,
            });

			return View(result);
        }

        public IActionResult Confirm(int id)
        {
			var hoaDon = _context.HoaDons.SingleOrDefault(hd => hd.MaHd == id);

			if (hoaDon == null)
			{
				// Nếu không tìm thấy hóa đơn, trả về lỗi hoặc thông báo
				TempData["Error"] = $"Không tìm thấy hóa đơn với mã {id}";
				return RedirectToAction("Index", "Cart");
			}

			// Cập nhật mã trạng thái
			hoaDon.MaTrangThai = 3;
            hoaDon.NgayGiao = DateTime.Now;

			// Lưu thay đổi vào cơ sở dữ liệu
			_context.SaveChanges();

			// Thông báo thành công
			TempData["Success"] = "Cảm ơn về đơn hàng của bạn!";
			return RedirectToAction("Index", "Home");
        }

        public IActionResult Remove(int id)
        {
            var pro = _context.HoaDons.SingleOrDefault(p => p.MaHd == id);

            pro.MaTrangThai = 3;
            _context.SaveChanges();
            return RedirectToAction("index", "home");
        }


    }
}
