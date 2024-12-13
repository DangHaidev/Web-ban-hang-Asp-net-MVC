using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web_ban_hang.Models.Entities;
using Web_ban_hang.Helpers;
using Web_ban_hang.ViewModels;
using Microsoft.EntityFrameworkCore;
using Web_ban_hang.Repository;

namespace Web_ban_hang.Controllers
{
	public class CustomerController : Controller
	{
		private readonly HDangShopContext db;
		private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        static string id = "dangd408";
		static string tempOTP = null;
		static string tempEmail="dang";
        public CustomerController(HDangShopContext context, IMapper mapper,IEmailSender email)
		{
			db = context;
			_mapper = mapper;
			_emailSender = email;
		}
		[HttpGet]
		public IActionResult SignUp()
		{
			return View();
		}

		[HttpPost]
		public IActionResult SignUp(RegisterVM model, IFormFile Hinh)
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
				TempData["Success"] = "Đăng ký tài khoản thành công!!";

				return RedirectToAction("SignIn", "Customer");

			}
			else
			{
				TempData["Error"] = "Có lỗi dữ liệu trong quá trình đăng ký, vui lòng thử lại";
				return View();
			}
		}



		#region Login
		[HttpGet]
		public IActionResult SignIn(string? ReturnUrl)
		{
			ViewBag.ReturnUrl = ReturnUrl;
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> SignIn(LoginVM model, string? ReturnUrl)
		{
			
			ViewBag.ReturnUrl = ReturnUrl;
			if (ModelState.IsValid)
			{
				var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName);
				if (khachHang == null)
				{
					ModelState.AddModelError("loi", "Không có khách hàng này");
					TempData["Error"] = "Không có khách hàng này";

                }
				else
				{
					if (!khachHang.HieuLuc)
					{
						ModelState.AddModelError("loi", "Tài khoản đã bị khóa. Vui lòng liên hệ Admin.");
                        TempData["Error"] = "Tài khoản đang bị khóa";

                    }
                    else
					{
						if (khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey))
						{
							ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
                            TempData["Error"] = "Sai thông tin đăng nhập";

                        }
                        else
						{
							id = khachHang.MaKh;
							var claims = new List<Claim> {
								new Claim(ClaimTypes.Email, khachHang.Email),
								new Claim(ClaimTypes.Name, khachHang.HoTen),
								new Claim("CustomerID", khachHang.MaKh),

								//claim - role động
								new Claim(ClaimTypes.Role, "Customer")
							};

							var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
							var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

							await HttpContext.SignInAsync(claimsPrincipal);

							if (Url.IsLocalUrl(ReturnUrl))
							{
								return Redirect(ReturnUrl);
							}
							else
							{
								return Redirect("/");
                            }
                        }
                    }
                }
            }

            
            return View();
		}
		#endregion

		[Authorize]
		public IActionResult Profile()
		{
            var customerss = db.KhachHangs.FirstOrDefault(p => p.MaKh == id);

            // Ánh xạ từ đối tượng KhachHang sang CustomerViewModel
            var customerViewModel = _mapper.Map<Customer>(customerss);
            return View(customerViewModel);
		}


        [HttpPost]
        public IActionResult Profile(Customer model, IFormFile? Hinh)
        {
            var cus = db.KhachHangs.SingleOrDefault(p => p.MaKh == model.MaKh);

            cus.MatKhau = model.MatKhau;
            cus.HoTen = model.HoTen;
            cus.DiaChi = model.DiaChi;
            cus.DienThoai = model.DienThoai;
            cus.Email = model.Email;
            cus.GioiTinh = model.GioiTinh;
            cus.NgaySinh = model.NgaySinh;
            cus.HieuLuc = true;

            if (Hinh != null)
            {
                cus.Hinh = Util.UploadHinh(Hinh, "KhachHang");
            }
            TempData["Success"] = "Cập nhật tài khoản thành công";
            db.SaveChanges();
            return RedirectToAction("index", "home");
        }

        [Authorize]
		public async Task<IActionResult> LogOut()
		{
			await HttpContext.SignOutAsync();
			return Redirect("/");
		}
		public  IActionResult ForgotPass( )
		{ return View(); }
            [HttpPost]
		public async Task<IActionResult> ForgotPass(ForgotPass model)
		{

            var cus = db.KhachHangs.FirstOrDefault(p => p.Email == model.Email);
			if(tempOTP != null)
			{
				if (cus != null)
				{
					if(tempOTP == model.Code)
					{
						tempEmail = cus.MaKh;
						return Redirect("RePass");
					}
					else
					{
						TempData["Error"] = "mã xác thực không chính xác";
						return View(model);
					} 
						
				}	
			}	
            if (cus == null)
            {
                TempData["Error"] = "Email chưa được đăng ký!";
                return View(model);
            }
            else
            {
                var receive = model.Email;
                var subject = "Mã OTP đổi lại mật khẩu";
                var message = tempOTP = GenerateOtp();

                await _emailSender.SendEmailAsync(receive, subject, message);

                TempData["Success"] = "Đã gửi mã về email của bạn!";

                return View(model);

            }
            //ViewBag.email = _email;
            return View();
		}
        public static string GenerateOtp()
        {
            Random random = new Random();
            int otp = random.Next(100000, 999999); // Sinh ra một số ngẫu nhiên từ 100000 đến 999999
            return otp.ToString();
        }

		public IActionResult RePass()
		{
			return View();
		}
		[HttpPost]
		public IActionResult RePass(rePass model)
		{
			var cus = db.KhachHangs.FirstOrDefault(p => p.MaKh == tempEmail);
			if (ModelState.IsValid)
			{
                cus.RandomKey = Util.GenerateRamdomKey();
                cus.MatKhau = model.Pass.ToMd5Hash(cus.RandomKey);

				db.SaveChanges();

				TempData["Success"] = "Đổi mật khẩu thành công";

				return Redirect("SignIn");
            }
			return View();
		}

    }
}
