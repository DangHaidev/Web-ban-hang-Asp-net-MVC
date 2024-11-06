using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web_ban_hang.Data;
using Web_ban_hang.Helpers;
using Web_ban_hang.ViewModels;

namespace Web_ban_hang.Controllers
{
	public class CustomerController : Controller
	{
		private readonly HDangShopContext db;
		private readonly IMapper _mapper;

		public CustomerController(HDangShopContext context, IMapper mapper)
		{
			db = context;
			_mapper = mapper;
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

				return RedirectToAction("Index", "HangHoa");
				//}
				//catch (Exception ex)
				//{
				//}

			}
			return RedirectToAction("/404");
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
				}
				else
				{
					if (!khachHang.HieuLuc)
					{
						ModelState.AddModelError("loi", "Tài khoản đã bị khóa. Vui lòng liên hệ Admin.");
					}
					else
					{
						if (khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey))
						{
							ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
						}
						else
						{
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
			return View();
		}

		[Authorize]
		public async Task<IActionResult> LogOut()
		{
			await HttpContext.SignOutAsync();
			return Redirect("/");
		}
	}
}
