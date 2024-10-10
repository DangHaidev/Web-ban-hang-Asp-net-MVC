using Microsoft.AspNetCore.Mvc;
using Web_ban_hang.Data;
using Web_ban_hang.ViewModels;

namespace Web_ban_hang.ViewComponents
{
    public class ListSaleViewComponent : ViewComponent
    {
        private readonly HDangShopContext db;

        public ListSaleViewComponent(HDangShopContext context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.HangHoas.Select(Loai => new ListSale
            {
                TenLoai = Loai.TenHh,
                Hinh = Loai.Hinh,
                DonGia = Loai.DonGia ?? 0,
            }).OrderBy(p => p.TenLoai);
            return View(data);
        }
    }
}
