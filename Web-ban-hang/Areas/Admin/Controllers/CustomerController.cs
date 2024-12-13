using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Web_ban_hang.Helpers;
using Web_ban_hang.Models.Entities;
using Web_ban_hang.ViewModels;

namespace Web_ban_hang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerController : Controller
    {
        private readonly HDangShopContext _context;
        private readonly IMapper _mapper;
    
        public CustomerController(HDangShopContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IActionResult Edit(string id)
        {
            var customerss = _context.KhachHangs.FirstOrDefault(p => p.MaKh == id);

            // Ánh xạ từ đối tượng KhachHang sang CustomerViewModel
            var customerViewModel = _mapper.Map<Customer>(customerss);

            return View(customerViewModel);
        }

        [HttpPost]
        public IActionResult Edit(Customer model, IFormFile? Hinh)
        {
            var cus = _context.KhachHangs.SingleOrDefault(p => p.MaKh == model.MaKh);

            cus.MatKhau = model.MatKhau;
            cus.HoTen = model.HoTen;
            cus.DiaChi = model.DiaChi;
            cus.DienThoai = model.DienThoai;
            cus.Email = model.Email;
            cus.GioiTinh = model.GioiTinh;
            cus.NgaySinh = model.NgaySinh;
            cus.HieuLuc = model.HieuLuc;

            if (Hinh != null)
            {
                cus.Hinh = Util.UploadHinh(Hinh, "KhachHang");
            }
            TempData["Success"] = "Sửa khách hàng thành công";
            _context.SaveChanges();
            return RedirectToAction("Customer","databoard");
        }

        public IActionResult LockAcc(string id)
        {
            var cus = _context.KhachHangs.SingleOrDefault(p => p.MaKh == id);
            if (cus.HieuLuc == true)
            {
                cus.HieuLuc = false;
            }
            else
            {
                cus.HieuLuc= true;
            }

            TempData["Success"] = "Cập nhật tài khoản khách hàng thành công";
            _context.SaveChanges();

            return RedirectToAction("Customer", "databoard");

        }
    }    
}
