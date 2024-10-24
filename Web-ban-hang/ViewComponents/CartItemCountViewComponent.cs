using Microsoft.AspNetCore.Mvc;
using Web_ban_hang.Controllers;
using Web_ban_hang.Services;

namespace Web_ban_hang.ViewComponents
{
    public class CartItemCountViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;

        public CartItemCountViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public IViewComponentResult Invoke()
        {
            // Gọi hàm GetCartItemCount từ CartController
            int cartItemCount = _cartService.GetCartItemCount();

            // Trả về giá trị để hiển thị trong view component
            return Content(cartItemCount.ToString());
        }
    }
}
