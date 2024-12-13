using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Web_ban_hang.Models;
using Web_ban_hang.Models.Entities;
using Web_ban_hang.Repository;
using Web_ban_hang.ViewModels;

namespace Web_ban_hang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HDangShopContext _context;
        private readonly IEmailSender _emailSender;
        public HomeController(ILogger<HomeController> logger, HDangShopContext ct,IEmailSender emailSender)
        {
            _logger = logger;
            _context = ct;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {

            ViewBag.SmartPhoneList = _context.HangHoas.Where(p => p.MaLoai == 1003).OrderByDescending(p=>p.MaHh).Take(8).Select(l => new { l.TenHh, l.MoTa,l.DonGia , l.Hinh,l.MaHh }).ToList();
            ViewBag.WatchList = _context.HangHoas.Where(p => p.MaLoai == 1000).OrderByDescending(p=>p.MaHh).Take(3).Select(l => new { l.TenHh, l.MoTa,l.DonGia , l.Hinh,l.MaHh }).ToList();
            ViewBag.LaptopList = _context.HangHoas.Where(p => p.MaLoai == 1001).OrderByDescending(p=>p.MaHh).Take(4).Select(l => new { l.TenHh, l.MoTa,l.DonGia , l.Hinh,l.MaHh }).ToList();
            ViewBag.CameraList = _context.HangHoas.Where(p => p.MaLoai == 1002).OrderByDescending(p=>p.MaHh).Take(2).Select(l => new { l.TenHh, l.MoTa,l.DonGia , l.Hinh,l.MaHh }).ToList();
            ViewBag.ValiList = _context.HangHoas.Where(p => p.MaLoai == 1007).OrderByDescending(p=>p.MaHh).Take(2).Select(l => new { l.TenHh, l.MoTa,l.DonGia , l.Hinh,l.MaHh }).ToList();

            ViewBag.TotalProduct = _context.HangHoas.Count();
            ViewBag.Customer = _context.KhachHangs.Count();

            return View();
        }
        [Route("/404")]        
        
        public IActionResult PageNotFound()
        {
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

        public IActionResult Contact() 
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Contact(Contact model) 
        {
            if (ModelState.IsValid)
            {

                var data = new GopY
                {
                    MaGy = DateTime.Now.ToString("ddMMyyHHmm"),
                    MaCd = 1,
                    HoTen = model.HoTen,
                    Email = model.Email,
                    NoiDung = model.NoiDung,
                    NgayGy = DateTime.Now,
                    CanTraLoi = true,
                };


                _context.Gopies.Add(data);
                _context.SaveChangesAsync();


                var receive = model.Email;
                var subject = "Thư cảm ơn";
                var message = "Cảm ơn bạn đã ghé thăm và để lại lời nhắn, chúng tôi sẽ sớm liên hệ cho bạn ngay!!! <33";

                await _emailSender.SendEmailAsync(receive, subject, message);
                TempData["Success"] = "Cảm ơn bạn đã gửi thông tin";
                return Redirect("contact");
            }
            else
            {
                TempData["Error"] = "Có lỗi trong quá trình, hãy thử lại";
            }
            return View();
        }
    }
}