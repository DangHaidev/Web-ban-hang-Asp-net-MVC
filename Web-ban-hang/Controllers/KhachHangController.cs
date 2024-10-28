using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Web_ban_hang.Data;
using Web_ban_hang.Helpers;
using Web_ban_hang.ViewModels;

namespace Web_ban_hang.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly HDangShopContext db;
		private readonly IMapper _mapper;

		public KhachHangController(HDangShopContext context, IMapper mapper) {
            db = context;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
		public IActionResult DangKy(RegisterVM model,IFormFile Hinh)
		{
            if (ModelState.IsValid)
            {
                //try
                //{
                    var khachHang = _mapper.Map<KhachHang>(model);
                    khachHang.RandomKey = Util.GenerateRamdomKey();
                    khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                    khachHang.HieuLuc = true; // xu li sau khi dung mail active
                    khachHang.VaiTro = 0;

                    if (Hinh != null)
                    {
                        khachHang.Hinh = Util.UploadHinh(Hinh, "KhachHang");
                    }

                    db.Add(khachHang);
                    db.SaveChanges();

                    return RedirectToAction("Index", "HangHoa");
                //}
                //catch (Exception ex)
                //{
                //}

            }
            return View();
        }
    }
}
