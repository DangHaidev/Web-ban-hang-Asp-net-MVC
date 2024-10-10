using Microsoft.AspNetCore.Mvc;
using Web_ban_hang.Data;
using Web_ban_hang.ViewModels;

namespace Web_ban_hang.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly HDangShopContext db;

        public MenuLoaiViewComponent(HDangShopContext context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(Loai => new MenuLoaiVM
            {
               MaLoai = Loai.MaLoai,
               TenLoai = Loai.TenLoai,
                SoLuong = Loai.HangHoas.Count
            }).OrderBy(p => p.TenLoai);          
           return View(data);
        }
    }
}