using Microsoft.AspNetCore.Mvc;
using Web_ban_hang.Data;
using Web_ban_hang.ViewModels;
using Web_ban_hang.Helpers;
using Web_ban_hang.Services;

namespace Web_ban_hang.Controllers
{
    public class CartController : Controller
    {
        private readonly HDangShopContext db; // Để sử dụng context
        private readonly ICartService _cartService; // Để sử dụng service

        // Constructor duy nhất kết hợp cả hai tham số
        public CartController(HDangShopContext context, ICartService cartService)
        {
            db = context;
            _cartService = cartService;
        }


        const string CART_KEY = "MYCART";
        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
        public IActionResult Index()
        {
            int cartItemCount = TotalCart();
            _cartService.SetCartItemCount(cartItemCount);
            return View(Cart);
        }
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item == null)
            {
                var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hangHoa == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    MaHh = hangHoa.MaHh,
                    TenHh = hangHoa.TenHh,
                    DonGia = hangHoa.DonGia ?? 0,
                    Hinh = hangHoa.Hinh ?? string.Empty,
                    SoLuong = quantity

                };
                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }
            HttpContext.Session.Set(CART_KEY,gioHang);
            return RedirectToAction("Index");
        }
        public IActionResult Remove(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(CART_KEY, gioHang);

            }
            return RedirectToAction("Index");

        }
        public IActionResult Minus(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                item.SoLuong--;
                HttpContext.Session.Set(CART_KEY, gioHang);

            }
            return RedirectToAction("Index");

        }
        public int TotalCart()
        {
            // nếu muốn trả về tất cả số lượng của các loại trong giổ hàg
            //var soluong = Cart.Sum(p => p.SoLuong);
            //return soluong;


            // trả về số loại hàng
            return Cart.Count;
        }      
    }
}
