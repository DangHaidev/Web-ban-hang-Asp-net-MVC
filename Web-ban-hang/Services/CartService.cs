namespace Web_ban_hang.Services
{
    public class CartService : ICartService
    {
        private int _cartItemCount;
        public int GetCartItemCount()
        {
            return _cartItemCount;
        }
        public void SetCartItemCount(int count)
        {
            _cartItemCount = count;
        }

    }
}
